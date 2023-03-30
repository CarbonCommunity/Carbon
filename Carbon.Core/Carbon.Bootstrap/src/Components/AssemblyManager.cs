using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Assembly;
using API.Contracts;
using API.Hooks;
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
	A MODULE is an optional part of Carbon, they can provide new functionality
	and/or are able to interact with the game directly. MODULES should only be
	created by the Carbon team as they CAN access sensitive parts of the framework.

	5) Extensions - API.Contracts.ICarbonExtension
	An EXTENSION are userland code that extend the framework with additional
	functionality but, for most cases EXTENSIONS shoulld not interact with the
	game directly. EXTENSIONS will also be passed by reference to the Rosylin
	compiler so plugins can use their feature set.

	6) Plugins
	Not applicable for now.
*/

internal sealed class AssemblyManagerEx : BaseMonoBehaviour, IAssemblyManager
{
	public List<string> LoadedComponents
	{ get; private set; } = new();

	public List<string> LoadedModules
	{ get; private set; } = new();

	public List<string> LoadedExtensions
	{ get; private set; } = new();

	public List<string> LoadedPlugins
	{ get; private set; } = new();

	public IReadOnlyList<string> References
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
					Assembly asm = _loader.Load(file, requester, directories)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (IsType<ICarbonComponent>(asm, out types))
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
					Assembly asm = _loader.Load(file, requester, directories)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (IsType<ICarbonModule>(asm, out types))
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
					Assembly asm = _loader.Load(file, requester, directories, true)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (IsType<ICarbonExtension>(asm, out types))
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
					Assembly asm = _loader.Load(file, requester, directories, false)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (IsType<Patch>(asm, out types))
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
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading hook '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading hook '{file}'");

			return null;
		}
#endif
	}

#if EXPERIMENTAL
	public Assembly LoadPlugin(string file, string requester = "unknown")
	{
		try
		{
			string[] directories =
			{
				Context.CarbonPlugins,
			};

			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, directories, true)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (IsType<ICarbonPlugin>(asm, out types))
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

					LoadedPlugins.Add(file);
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
#endif

	private bool IsType<T>(Assembly assembly, out IEnumerable<Type> output)
	{
		try
		{
			Type @base = typeof(T) ?? throw new Exception();
			output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));
			return output.Count() > 0;
		}
		catch (System.Exception)
		{
			output = new List<Type>();
			return false;
		}
	}

	private static readonly IReadOnlyList<string> _knownLibs = new List<string>() {
		"mscorlib",
		"netstandard",

		"System.Core",
		"System.Data",
		"System.Drawing",
		"System.Memory",
		"System.Net.Http",
		"System.Runtime",
		"System.Xml.Linq",
		"System.Xml.Serialization",
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

		"Fleck", // websocket server
		"Newtonsoft.Json",

		"Rust.Data",
		"Rust.FileSystem",
		"Rust.Global",
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
