using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using API.Abstracts;
using API.Assembly;
using API.Commands;
using Carbon.Compat;
using Loaders;
using Utility;

namespace Components;
#pragma warning disable IDE0051

internal sealed class AssemblyManager : CarbonBehaviour, IAssemblyManager
{
	private LibraryLoader _library;

	public IReadOnlyList<string> RefBlacklist => _blacklistLibs;
	public IReadOnlyList<string> RefWhitelist => _whitelistLibs;

	public IAddonManager Components { get; private set; }
	public IExtensionManager Extensions { get; private set; }
	public IAddonManager Hooks { get; private set; }
	public IAddonManager Modules { get; private set; }

	private void Awake()
	{
		_library = LibraryLoader.GetInstance();

		gameObject.AddComponent<EventManager>();
		Components = gameObject.AddComponent<ComponentManager>();
		Extensions = gameObject.AddComponent<ExtensionManager>();
		Hooks = gameObject.AddComponent<HookManager>();
		Modules = gameObject.AddComponent<ModuleManager>();
		gameObject.AddComponent<CompatManager>();

#if DEBUG
		Carbon.Bootstrap.Commands.RegisterCommand(new Command.RCon
		{
			Name = "c.assembly",
			Callback = arg => CMDAssemblyInfo(arg)
		}, out _);
#endif
	}

	public byte[] Read(string file, string[] directories = null)
	{
		byte[] raw = default;

		Assembly assembly = _library.GetDomain().GetAssemblies().FirstOrDefault(asm => asm.GetName().Name.Equals(file));

		if (assembly != null && !assembly.IsDynamic)
		{
			if (assembly.Location != string.Empty)
			{
				raw = File.ReadAllBytes(assembly.Location);

				if (raw != null)
				{
					return raw;
				}
			}
		}

		foreach(var extension in Extensions.Loaded.Values)
		{
			if (Path.GetFileName(extension.Key) != file)
			{
				continue;
			}

			raw = Extensions.Read(file);
			if (raw != null)
			{
				return raw;
			}
		}

		foreach (string expr in _blacklistLibs)
		{
			if (Regex.IsMatch(file, expr))
			{
				break;
			}

			IAssemblyCache result = _library.ResolveAssembly(file, $"{this}", directories);

			if (result != null && result.Raw != null)
			{
				return result.Raw;
			}
		}

		return null;
	}

	public bool IsType<T>(Assembly assembly, out IEnumerable<Type> output)
	{
		try
		{
			output = assembly.GetTypes().Where(type => typeof(T).IsAssignableFrom(type));
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
			output = default;
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

	private static readonly IReadOnlyList<string> _blacklistLibs = [
		@"^Carbon\.Bootstrap|Preloader$",
		@"^Carbon\..+_\d{4}\.\d{2}\.\d{2}\.\d{4}$",
	];

	private static readonly IReadOnlyList<string> _whitelistLibs = [
		"mscorlib",
		"netstandard",

		"System.Core",
		"System.Data",
		"System.Drawing",
		"System.Globalization",
		"System.Management",
		"System.Net.Http",
		"System.Memory",
		"System.Runtime.CompilerServices.Unsafe",
		"System.Runtime",
		"System.Threading.Tasks.Extensions",
		"System.Xml.Linq",
		"System.Xml.Serialization",
		"System.Xml",
		"System",

		"Carbon",
		"Carbon.Common",
		"Carbon.SDK",
		"Carbon.Test",
		"Carbon.Startup",
		"Carbon.Profiler",
		"Carbon.Polyfills",

		"MySql.Data",
		"MySqlConnector",
		"protobuf-net.Core",
		"protobuf-net",
		"websocket-sharp",

		"Assembly-CSharp-firstpass",
		"Assembly-CSharp",

		"ZString",
		"Fleck",
		"Newtonsoft.Json",
		"Ionic.Zip.Reduced",

		"Cronos",
		"BoomlagoonJSON",
		"Facepunch.BurstCloth",
		"Facepunch.Console",
		"Facepunch.Network",
		"Facepunch.Rcon",
		"Facepunch.Raknet",
		"Facepunch.Sqlite",
		"Facepunch.System",
#if UNIX
		"Facepunch.Steamworks.Posix",
#else
		"Facepunch.Steamworks.Win64",
#endif
		"Facepunch.SteamNetworking",
		"Facepunch.Unity",
		"Facepunch.UnityEngine",
		"Facepunch.Nexus",
		"Facepunch.Ping",
		"UniTask",
		"UniTask.Addressables",
		"UniTask.DOTween",
		"UniTask.Linq",
		"UniTask.TextMeshPro",

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
		"UnityEngine.AssetBundleModule",
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
	];
}
