using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using API.Assembly;
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

internal sealed class AssemblyManager : BaseMonoBehaviour, IAssemblyManager
{
	private LibraryLoader _library;

	public IReadOnlyList<string> References
	{ get => _knownLibs; }

	public IAssemblyTypeManager Components
	{ get => gameObject.GetComponent<ComponentManager>(); }

	public IAssemblyTypeManager Extensions
	{ get => gameObject.GetComponent<ExtensionManager>(); }

	public IAssemblyTypeManager Hooks
	{ get => gameObject.GetComponent<HookManager>(); }

	public IAssemblyTypeManager Modules
	{ get => gameObject.GetComponent<ModuleManager>(); }

#if EXPERIMENTAL
	public IAssemblyTypeManager Plugins
	{ get => gameObject.GetComponent<PluginManager>(); }
#endif

	private void Awake()
	{
		_library = LibraryLoader.GetInstance();

		gameObject.AddComponent<ComponentManager>();
		gameObject.AddComponent<ExtensionManager>();
		gameObject.AddComponent<HookManager>();
		gameObject.AddComponent<ModuleManager>();
#if EXPERIMENTAL
		gameObject.AddComponent<PluginManager>();
#endif
	}

	public byte[] Read(string file)
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

		if (Extensions.Loaded.Contains(file))
		{
			raw = Extensions.Read(file);
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

	public bool IsType<T>(Assembly assembly, out IEnumerable<Type> output)
	{
		try
		{
			Type @base = typeof(T) ?? throw new Exception();
			output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));

			// NOTE: If we have issues with IsType<> test the following implementation:
			// if(@base.IsInterface)
			// {
			// 	output = assembly.GetTypes().Where(type => type.GetInterfaces().Contains(@base));
			// }
			// else
			// {
			// 	output = assembly.GetTypes().Where(type => @base.IsAssignableFrom(type));
			// }

			return output.Count() > 0;
		}
		catch
#if DEBUG
		(Exception ex)
		{
			Logger.Error($"Failed IsType<{typeof(T).FullName}>", ex);
#else
		{
#endif
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
		"System.Globalization",
		"System.Management",
		"System.Memory",
		"System.Net.Http",
		"System.Runtime",
		"System.Xml.Linq",
		"System.Xml.Serialization",
		"System.Xml",
		"System",

		"Carbon.Common",
		"Carbon.SDK",

		"0Harmony", // this needs to be injected only when
					// IHarmony is defined
					
		"MySql.Data", // v6.9.5.0
		"protobuf-net.Core",
		"protobuf-net",
		"websocket-sharp", // ws client/server

		"Assembly-CSharp-firstpass",
		"Assembly-CSharp",

		"Fleck", // bundled with rust (websocket server)
		"Newtonsoft.Json", // bundled with rust

		"Facepunch.BurstCloth",
		"Facepunch.Console",
		"Facepunch.Network",
		"Facepunch.Rcon",
		"Facepunch.Raknet",
		"Facepunch.Sqlite",
		"Facepunch.System",
		"Facepunch.Unity",
		"Facepunch.UnityEngine",

		"Rust.Data",
		"Rust.FileSystem",
		"Rust.Global",
		"Rust.Harmony",
		"Rust.Localization",
		"Rust.Platform.Common",
		"Rust.Platform",
		"Rust.UI",
		"Rust.Workshop",
		"Rust.World",

		"Unity.Mathematics",
		"Unity.Timeline",
		"UnityEngine.AIModule",
		"UnityEngine.AnimationModule",
		"UnityEngine.CoreModule",
		"UnityEngine.ImageConversionModule",
		"UnityEngine.ParticleSystemModule",
		"UnityEngine.PhysicsModule",
		"UnityEngine.SharedInternalsModule",
		"UnityEngine.TerrainModule",
		"UnityEngine.TerrainPhysicsModule",
		"UnityEngine.TextRenderingModule",
		"UnityEngine.UI",
		"UnityEngine.UIModule",
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
