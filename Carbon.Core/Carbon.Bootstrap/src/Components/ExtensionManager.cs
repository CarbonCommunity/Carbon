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

internal sealed class ExtensionManager : BaseTypeManager
{
	/*
	 * CARBON EXTENSIONS
	 * API.Contracts.ICarbonExtension
	 * 
	 * An assembly to be considered as a Carbon Extension must:
	 *   1. Implement the ICarbonExtension interface
	 *   2. Must not change directly with the world
	 *   3. Provide additional functionality such as new features or services
	 *
	 * Carbon extensions are different from Oxide extensions, in Carbon extensions
	 * are "libraries" and cannot access features such as hooks or change the
	 * world, either directly or using reflection.
	 *
	 */
	private readonly string[] _directories =
	{
		Context.CarbonExtensions,
	};

	internal void Awake()
	{
		Carbon.Bootstrap.Watcher.Watch(new WatchFolder
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Utility.Context.CarbonExtensions,

			OnFileCreated = (sender, file) =>
			{
				Carbon.Bootstrap.AssemblyEx.Extensions.Load(
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

					if (AssemblyManager.IsType<ICarbonExtension>(asm, out types))
					{
						Logger.Debug($"Loading extension from file '{file}'");

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonExtension extension)
									throw new NullReferenceException();
								Logger.Debug($"A new instance of '{extension}' created");

								extension.OnLoaded(args: new EventArgs());
							}
							catch (Exception e)
							{
								Logger.Error($"Failed to instantiate extension from type '{type}'", e);
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
			Logger.Error($"Error while loading extension from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading extension '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading extension '{file}'");

			return null;
		}
#endif
	}
}
