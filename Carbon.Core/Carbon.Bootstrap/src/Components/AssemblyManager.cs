using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using API.Abstracts;
using API.Assembly;
using API.Commands;
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

internal sealed class AssemblyManager : CarbonBehaviour, IAssemblyManager
{
	private LibraryLoader _library;

	public IReadOnlyList<string> RefBlacklist
	{ get => _blacklistLibs; }

	public IReadOnlyList<string> RefWhitelist
	{ get => _whitelistLibs; }

	public IAddonManager Components
	{ get => gameObject.GetComponent<ComponentManager>(); }

	public IAddonManager Extensions
	{ get => gameObject.GetComponent<ExtensionManager>(); }

	public IAddonManager Hooks
	{ get => gameObject.GetComponent<HookManager>(); }

	public IAddonManager Modules
	{ get => gameObject.GetComponent<ModuleManager>(); }

#if EXPERIMENTAL
	public IAddonManager Plugins
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

#if DEBUG
		Carbon.Bootstrap.Commands.RegisterCommand(new Command.RCon
		{
			Name = "c.assembly",
			Callback = (arg) => CMDAssemblyInfo(arg)
		}, out string reason);
#endif
	}

	public byte[] Read(string file, string[] directories = null)
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

		// if (_whitelistLibs.Contains(file))
		// {
		// 	IAssemblyCache result = _library.ResolveAssembly(file, $"{this}");
		// 	if (result.Raw != null) return result.Raw;
		// }

		foreach (string expr in _blacklistLibs)
		{
			if (Regex.IsMatch(file, expr)) break;
			IAssemblyCache result = _library.ResolveAssembly(file, $"{this}", directories);
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

	private void CMDAssemblyInfo(Command.Args arg)
	{
		int count = 0;
		TextTable table = new();

		table.AddColumns("#", "Assembly", "Version", "Dynamic", "Location");

		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			table.AddRow(
				$"{count++:n0}",
				assembly.GetName().Name,
				$"{assembly.GetName().Version}",
				$"{assembly.IsDynamic}",
				(assembly.IsDynamic) ? string.Empty : assembly.Location
			);
		}

		arg.ReplyWith(table.ToString());
	}

	private static readonly IReadOnlyList<string> _blacklistLibs = new List<string>() {
		@"^Carbon$",
		@"^Carbon\.Bootstrap|Preloader$",
		@"^Carbon\..+_\d{4}\.\d{2}\.\d{2}\.\d{4}$",
	};

	private static readonly IReadOnlyList<string> _whitelistLibs = new List<string>() {
		"mscorlib",
		"netstandard",

		"System.Core",
		"System.Data",
		"System.Drawing",
		"System.Globalization",
		"System.Management",
		"System.Net.Http",
		"System.Runtime",
		"System.Threading.Tasks.Extensions",
		"System.Xml.Linq",
		"System.Xml.Serialization",
		"System.Xml",
		"System",

		"Carbon.Common",
		"Carbon.SDK",

		"MySql.Data", // v6.9.5.0
		"protobuf-net.Core",
		"protobuf-net",
		"websocket-sharp", // ws client/server

		"Assembly-CSharp-firstpass",
		"Assembly-CSharp",

		"Fleck", // bundled with rust (websocket server)
		"Newtonsoft.Json", // bundled with rust

		"Ionic.Zip.Reduced",

		"Facepunch.BurstCloth",
		"Facepunch.Console",
		"Facepunch.Network",
		"Facepunch.Rcon",
		"Facepunch.Raknet",
		"Facepunch.Sqlite",
		"Facepunch.System",
		"Facepunch.Unity",
		"Facepunch.UnityEngine",
		"Facepunch.Nexus",

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
		"Rust.Clans",
		"Rust.Clans.Local",

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
