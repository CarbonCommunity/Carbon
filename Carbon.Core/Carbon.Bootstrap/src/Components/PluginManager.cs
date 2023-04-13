using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Plugins;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class PluginManager : BaseAssemblyType
{
	private readonly string[] _directories =
	{
		Context.CarbonPlugins,
	};

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly Load(string file, string requester = null)
	{
		if (requester is null)
		{
			MethodBase caller = new StackFrame(1).GetMethod();
			requester = $"{caller.DeclaringType}.{caller.Name}";
		}

		try
		{
			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, _directories, AssemblyManager.References)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (AssemblyManager.IsType<ICarbonPlugin>(asm, out types))
					{
						Logger.Debug($"Loading plugin from file '{file}'");

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonPlugin plugin)
									throw new Exception($"Failed to create an 'ICarbonPlugin' instance from '{type}'");

								Logger.Debug($"A new instance of '{plugin}' was created");

								// Carbon.Plugins.PluginBase.Logger
								PropertyInfo b = plugin.GetType().GetProperty("Logger",
									BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
									?? throw new Exception("Logger field not found on assembly");

								// Carbon.Logger
								Type c = HarmonyLib.AccessTools.TypeByName("Carbon.Logger")
									?? throw new Exception("Logger type not found");

								b.SetValue(plugin, Activator.CreateInstance(c));

								plugin.Initialize("nothing for now");
								plugin.OnLoaded(args: new EventArgs());
							}
							catch (Exception e)
							{
								Logger.Error($"Failed to instantiate plugin from type '{type}'", e);
								continue;
							}
						}
					}
					else
					{
						throw new Exception("Unsupported assembly type");
					}

					Loaded.Add(file);
					return asm;

				// case ".drm"
				// 	LoadFromDRM();
				// 	break;

				default:
					throw new Exception("File extension not supported");
			}
		}
		catch (ReflectionTypeLoadException)
		{
			Logger.Error($"Error while loading plugin from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading plugin '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading plugin '{file}'");

			return null;
		}
#endif
	}
}
