using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Assembly;
using API.Contracts;
using Loaders;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

/*
	Types of assemblies:

	1) Library
	A LIBRARY is an external dependency which is required by a loading assembly
	to be executed, this is the same definition as C# reference.

	2) Component - API.Contracts.ICarbonComponent
	A COMPONENT is a core assembly for Carbon, core assembliess are always required
	for Carbon to work. Examples are Preloader, Bootstrap, Common and API.

	3) Hooks - API.Hooks.Patch
	A HOOK is a set of harmony patche which trigger events.

	4) Module - API.Contracts.ICarbonModule
	A MODULE is an optional feature for Carbon which is not required for the core
	functionality of the framework.

	5) Extensions - API.Contracts.ICarbonExtension
	An EXTENSION is an assembly just like a plugin with the difference that it will
	be passed as a reference to the Rosylin compiler so plugins can use them.

	6) Plugins
	Not applicable for now.
*/

internal sealed class AssemblyManagerEx : BaseMonoBehaviour, IAssemblyManager
{
	public List<string> LoadedModules
	{ get; private set; } = new();

	public List<string> LoadedComponents
	{ get; private set; } = new();

	public List<string> LoadedExtensions
	{ get; private set; } = new();

	public string[] References
	{ get => _knownLibs; }


	private LibraryLoader _library;
	private AssemblyLoader _loader;


	private void Awake()
	{
		_library = new LibraryLoader();
		_loader = new AssemblyLoader();
	}

	private void OnEnable()
	{
		_library.RegisterDomain(AppDomain.CurrentDomain);
	}

	private void OnDisable()
	{
		_library.UnregisterDomain();
	}

	private void OnDestroy()
	{
		_library.Dispose();
	}

	public byte[] Read(string file, string requester = "unknown")
	{
		byte[] raw = default;

		Assembly assembly = _library.GetDomain().GetAssemblies()
			.FirstOrDefault(asm => asm.GetName().Name.Equals(file));
		if (assembly != null && !assembly.IsDynamic)
		{
			if (assembly.Location != string.Empty)
			{
				raw = File.ReadAllBytes(assembly.Location);
				if (raw != null) return raw;
			}
		}

		if (LoadedExtensions.Contains(file))
		{
			raw = _loader.ReadFromCache(file).Raw;
			if (raw != null) return raw;
		}

		if (_knownLibs.Contains(file))
		{
			IAssemblyCache result = _library.ResolveAssembly(file, $"{this}");
			if (result.Raw != null) return result.Raw;
		}

		Logger.Warn($"Unable to get byte[] for '{file}'");
		return default;
	}

	public Assembly LoadComponent(string file, string requester = "unknown")
	{
		try
		{
			string[] directories =
			{
				Context.CarbonManaged,
			};

			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, directories).Assembly;

					if (IsComponent(asm, out types))
					{
						Logger.Debug($"Loading component from file '{file}'");

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonComponent component)
									throw new NullReferenceException();
								Logger.Debug($"A new instance of '{component}' created");

								component.Initialize("nothing for now");
								component.OnLoaded(args: new EventArgs());
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

					LoadedComponents.Add(file);
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
			Logger.Error($"Either the file is corrupt or has it's from an unsuported version.");
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

	public Assembly LoadModule(string file, string requester = "unknown")
	{
		try
		{
			string[] directories =
			{
				Context.CarbonManaged,
			};

			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, directories).Assembly;

					if (IsModule(asm, out types))
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

					LoadedModules.Add(file);
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
			Logger.Error($"Either the file is corrupt or has it's from an unsuported version.");
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

	public Assembly LoadExtension(string file, string requester = "unknown")
	{
		try
		{
			string[] directories =
			{
				Context.CarbonExtensions,
			};

			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, directories).Assembly;

					if (IsExtension(asm, out types))
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

					LoadedExtensions.Add(file);
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
			Logger.Error($"Either the file is corrupt or has it's from an unsuported version.");
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

	public Assembly LoadHook(string file, string requester = "unknown")
	{
		try
		{
			string[] directories =
			{
				Context.CarbonHooks,
			};

			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, directories).Assembly;

					if (IsHook(asm, out types))
					{
						Logger.Debug($"Loading hooks file '{file}'");
						// TODO: Integrate part of HookManager here
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
			Logger.Error($"Error while loading hooks from '{file}'.");
			Logger.Error($"Either the file is corrupt or has it's from an unsuported version.");
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

	private bool IsComponent(Assembly assembly, out IEnumerable<Type> output)
	{
		Type @base = typeof(API.Contracts.ICarbonComponent);
		output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));
		return (output.Count() > 0);
	}

	private bool IsModule(Assembly assembly, out IEnumerable<Type> output)
	{
		Type @base = typeof(API.Contracts.ICarbonModule);
		output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));
		return (output.Count() > 0);
	}

	private bool IsExtension(Assembly assembly, out IEnumerable<Type> output)
	{
		Type @base = typeof(API.Contracts.ICarbonExtension);
		output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));
		return (output.Count() > 0);
	}

	private bool IsHook(Assembly assembly, out IEnumerable<Type> output)
	{
		Type @base = typeof(API.Hooks.Patch);
		output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));
		return (output.Count() > 0);
	}

	public static readonly string[] _knownLibs = {
		"mscorlib",
		"netstandard",

		"System.Core",
		"System.Data",
		"System.Drawing",
		"System.Memory",
		"System.Net.Http",
		"System.Runtime",
		"System.Xml.Linq",
		"System.Xml",
		"System",

		"Carbon.Common",
		"Carbon.SDK",

		"protobuf-net",
		"protobuf-net.Core",
		"websocket-sharp",

		"Assembly-CSharp-firstpass",
		"Assembly-CSharp",
		"Facepunch.Console",
		"Facepunch.Network",
		"Facepunch.Rcon",
		"Facepunch.Sqlite",
		"Facepunch.System",
		"Facepunch.Unity",
		"Facepunch.UnityEngine",
		"Fleck",
		"Newtonsoft.Json",
		"Rust.Data",
		"Rust.FileSystem",
		"Rust.Global",
		// "Rust.Harmony",
		"Rust.Localization",
		"Rust.Platform.Common",
		"Rust.Platform",
		"Rust.Workshop",
		"Rust.World",
		"UnityEngine.AIModule",
		"UnityEngine.CoreModule",
		"UnityEngine.ImageConversionModule",
		"UnityEngine.PhysicsModule",
		"UnityEngine.SharedInternalsModule",
		"UnityEngine.TerrainModule",
		"UnityEngine.TerrainPhysicsModule",
		"UnityEngine.TextRenderingModule",
		"UnityEngine.UI",
		"UnityEngine.UnityWebRequestAssetBundleModule",
		"UnityEngine.UnityWebRequestAudioModule",
		"UnityEngine.UnityWebRequestModule",
		"UnityEngine.UnityWebRequestTextureModule",
		"UnityEngine.UnityWebRequestWWWModule",
		"UnityEngine.VehiclesModule",
		"UnityEngine",

#if WIN
		"Facepunch.Steamworks.Win64",
#elif UNIX
		"Facepunch.Steamworks.Posix",
#endif
	};
}
