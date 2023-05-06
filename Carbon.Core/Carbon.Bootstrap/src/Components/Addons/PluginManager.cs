using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Assembly;
using API.Events;
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

internal sealed class PluginManager : AddonManager
{
	private readonly string[] _directories =
	{
		Context.CarbonPlugins,
	};

#if EXPERIMENTAL
	internal void Awake()
	{
		Carbon.Bootstrap.Watcher.Watch(new WatchItem
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Context.CarbonPlugins,

			OnFileCreated = (sender, file) =>
			{
				Carbon.Bootstrap.AssemblyEx.Plugins.Load(
					Path.GetFileName(file), $"{typeof(FileWatcherManager)}");
			},
		});
	}
#endif

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
					Assembly asm = _loader.Load(file, requester, _directories, null, AssemblyManager.RefWhitelist)?.Assembly
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
								Hydrate(asm, plugin);

								plugin.Awake(EventArgs.Empty);
								plugin.OnLoaded(EventArgs.Empty);

								Carbon.Bootstrap.Events
									.Trigger(CarbonEvent.PluginLoaded, new CarbonEventArgs(file));

								_loaded.Add(new() { Addon = plugin, File = file });
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

	internal override void Hydrate(Assembly assembly, ICarbonAddon addon)
	{
		base.Hydrate(assembly, addon);

		BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		addon.GetType().GetProperty("Logger", flags)?.SetValue(addon,
			Activator.CreateInstance(HarmonyLib.AccessTools.TypeByName("Carbon.Logger") ?? null));
	}
}
