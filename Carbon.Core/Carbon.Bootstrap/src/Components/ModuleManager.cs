using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Assembly;
using API.Contracts;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class ModuleManager : BaseAssemblyType
{
	/*
	 * CARBON MODULES
	 * API.Contracts.ICarbonModule
	 * 
	 * An assembly to be considered as a Carbon Module must:
	 *   1. Be optional
	 *   2. Implement the ICarbonModule interface
	 *   3. Provide additional functionality such as new features or services
	 *
	 * Carbon modules can be compared to Oxide Extensions, they can be created
	 * by anyone and can change and/or interact with the world as any other user
	 * plugin can.
	 *
	 */
	private readonly string[] _directories =
	{
		Context.CarbonModules,
	};

	internal void Awake()
	{
		Carbon.Bootstrap.Watcher.Watch(new WatchFolder
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Utility.Context.CarbonModules,

			OnFileCreated = (sender, file) =>
			{
				Carbon.Bootstrap.AssemblyEx.Modules.Load(
					Path.GetFileName(file), $"{typeof(FileWatcherManager)}");
			},
		});
	}

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

					if (AssemblyManager.IsType<ICarbonModule>(asm, out types))
					{
						Logger.Debug($"Loading module from file '{file}'");

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonModule module)
									throw new NullReferenceException();
								Logger.Debug($"A new instance of '{module}' created");

								module.Initialize("nothing for now");
								module.OnLoaded(args: new EventArgs());
							}
							catch (Exception e)
							{
								Logger.Error($"Failed to instantiate module from type '{type}'", e);
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
			Logger.Error($"Error while loading module from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading module '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading module '{file}'");

			return null;
		}
#endif
	}
}
