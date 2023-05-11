using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Assembly;
using API.Events;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class ComponentManager : AddonManager
{
	/*
	 * CARBON COMPONENTS
	 * API.Contracts.ICarbonComponent
	 * 
	 * An assembly to be considered as a Carbon Component must:
	 *   1. Implement the ICarbonComponent interface
	 *   2. Be developed and maintained by the Carbon team
	 *
	 * Components are the basis of the Carbon framework and should be distributed
	 * with the standard release package.
	 *
	 */
	private readonly string[] _directories =
	{
		Context.CarbonManaged,
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
					Assembly asm = _loader.Load(file, requester, _directories, null, null)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (AssemblyManager.IsType<ICarbonComponent>(asm, out types))
					{
						Logger.Debug($"Loading component from file '{file}'");

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonComponent component)
									throw new NullReferenceException();

								Logger.Debug($"A new instance of '{component}' created");

								component.Awake(EventArgs.Empty);
								component.OnLoaded(EventArgs.Empty);

								Carbon.Bootstrap.Events
									.Trigger(CarbonEvent.ComponentLoaded, new CarbonEventArgs(file));

								_loaded.Add(new() { Addon = component, File = file });
							}
							catch (Exception e)
							{
								Logger.Error($"Failed to instantiate component from type '{type}'", e);
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
			Logger.Error($"Error while loading component from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading component '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading component '{file}'");

			return null;
		}
#endif
	}
}
