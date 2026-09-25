using Carbon.Compat;
using Carbon.Compat.Converters;
using Carbon.Compat.Patches.Harmony;
using Carbon.Compat.Patches.Oxide;
using Carbon.Jobs;
using Carbon.Managers;
using Oxide.Game.Rust;
using ProtoBuf.Meta;

namespace Oxide.Compatibility;

/// <summary>
/// Entry point of the Oxide compatibility package, loaded by Carbon.Bootstrap before Carbon.dll when present.
/// Everything Oxide-specific plugs into Carbon's extension points from here; Carbon itself knows nothing about Oxide.
/// </summary>
public class OxidePackage : ICarbonComponent
{
	public static bool IsLoaded { get; private set; }

	public void Awake(EventArgs args)
	{
	}

	public void OnLoaded(EventArgs args)
	{
		if (IsLoaded)
		{
			return;
		}

		IsLoaded = true;

		if (IsOxideInstalled())
		{
			return;
		}

		RegisterCompiler();
		RegisterAssemblies();
		RegisterHooks();
		RegisterCommands();
		RegisterAdmin();

		Services.Events.Subscribe(CarbonEvent.LibrariesInstalled, _ => OnLibrariesInstalled());

		// Oxide's PluginManager events (OnPluginAdded/OnPluginRemoved)
		Services.Events.Subscribe(CarbonEvent.PluginLoaded, args =>
		{
			if (args is CarbonEventArgs { Payload: Plugin plugin }) Interface.Oxide.RootPluginManager.AddPlugin(plugin);
		});
		Services.Events.Subscribe(CarbonEvent.PluginUnloaded, args =>
		{
			if (args is CarbonEventArgs { Payload: Plugin plugin }) Interface.Oxide.RootPluginManager.RemovePlugin(plugin);
		});
		Services.Events.Subscribe(CarbonEvent.HookValidatorRefreshed, _ => InstallCore());

		Logger.Log("Loaded Oxide compatibility package");
	}

	public void OnUnloaded(EventArgs args)
	{
	}

	public void OnEnable(EventArgs args)
	{
	}

	public void OnDisable(EventArgs args)
	{
	}

	/// <summary>A real Oxide install (patched server assembly) can't run next to Carbon, stop the server if found.</summary>
	private static bool IsOxideInstalled()
	{
		try
		{
			if (Type.GetType("Oxide.Core.Interface, Oxide.Core") is null)
			{
				return false;
			}

			Logger.Log(Environment.NewLine +
				@"                                                          " + Environment.NewLine +
				@"  ________ _______ ______ _______ _______ _______ _______ " + Environment.NewLine +
				@" |  |  |  |   _   |   __ \    |  |_     _|    |  |     __|" + Environment.NewLine +
				@" |  |  |  |       |      <       |_|   |_|       |    |  |" + Environment.NewLine +
				@" |________|___|___|___|__|__|____|_______|__|____|_______|" + Environment.NewLine +
				@"                                                          " + Environment.NewLine +
				@"    WE HAVE DETECTED YOUR SERVER IS STILL PATCHED WITH    " + Environment.NewLine +
				@"    OXIDE. CARBON WILL NOT WORK IN THIS ENVIRONMENT.      " + Environment.NewLine +
				@"                                                          " + Environment.NewLine +
				@"    PLEASE VERIFY YOUR GAME FILES WITH STEAMCMD THEN      " + Environment.NewLine +
				@"    REBOOT THE SERVER.                                    " + Environment.NewLine +
				@"                                                          " + Environment.NewLine +
				@"    THIS SERVER WILL BE TERMINATED IN 60 SECONDS.         " + Environment.NewLine +
				@"    THANK YOU <3                                          " + Environment.NewLine +
				@"                                                          " + Environment.NewLine
			);

			Thread.Sleep(60000);
			Application.Quit();
			return true;
		}
		catch (Exception e)
		{
			Logger.Error("Unable to assert assembly status.", e);
			return false;
		}
	}

	/// <summary>Lets the compiler accept and translate Oxide plugins.</summary>
	private static void RegisterCompiler()
	{
		ModLoader.PluginNamespaces.Add("Oxide.Plugins");
		ModLoader.PluginBaseTypes.Add(typeof(RustPlugin));
		ModLoader.PluginBaseTypes.Add(typeof(CovalencePlugin));

		ScriptCompilationThread.SourceTransformers.Add(OxideSourceTranslator.Transform);
		ScriptCompilationThread.ConditionalSymbols.Add("OXIDE_PUBLICIZED");
	}

	/// <summary>Precompiled Oxide extensions and HarmonyMods built against Oxide get their references remapped.</summary>
	private static void RegisterAssemblies()
	{
		// Real Oxide assemblies must never load next to Carbon
		LibraryLoader.Blacklist.Add(@"^Oxide\..+$");

		Services.Assemblies.Converters.Add(new OxideExtensionConverter());

		HarmonyConverter.AdditionalPatches.Add(new OxideTypeRef());
		HarmonyConverter.AdditionalPatches.Add(new OxideILSwitch());
		HarmonyPatchProcessor.PatchWhitelist.string_blacklist.Add("Oxide.Core.OxideMod");
		HarmonyPatchProcessor.PatchWhitelist.string_blacklist.Add("Oxide.Core");
	}

	private static void RegisterHooks()
	{
		PatchManager.HookAssemblies.Add("Carbon.Hooks.Oxide.dll");
		Carbon.Hooks.Updater.RemoteFiles.Add("carbon/managed/hooks/Carbon.Hooks.Oxide.dll");
	}

	/// <summary>Command methods taking an IPlayer get the caller as a covalence player.</summary>
	private static void RegisterCommands()
	{
		CommandLibrary.CallerFactories[typeof(IPlayer)] = player =>
		{
			if (player == null)
			{
				return new RustPlayer { IsServer = true };
			}

			var iplayer = player.AsIPlayer();
			iplayer.IsServer = false;
			return iplayer;
		};
	}

	private static void RegisterAdmin()
	{
#if !MINIMAL
		Carbon.Modules.AdminModule.PluginsTab.ExternalVendorFactory = () => new UModVendor();
		Carbon.Modules.AdminModule.PluginsTab.Installed.DefaultTags.Add("oxide");
		Carbon.Modules.ImageDatabaseModule.DefaultImages["umodlogo"] = "https://cdn.carbonmod.gg/content/umod-logo.png";
		Carbon.Modules.ImageDatabaseModule.DefaultImages["umod_hero"] = "https://cdn.carbonmod.gg/content/umod_hero.png";

		// The vendor cache is protobuf-serialized through the base vendor type
		RuntimeTypeModel.Default[typeof(Carbon.Modules.AdminModule.PluginsTab.Vendor)].AddSubType(101, typeof(UModVendor));
#endif
	}

	private static void OnLibrariesInstalled()
	{
		Interface.Initialize();

		// Keep covalence players in sync with the permission user data
		Community.Runtime.Permission.OnUserRefreshed += (player, _) =>
		{
			var iplayer = player.AsIPlayer();
			iplayer.Name = player.displayName.Replace("{", "{{").Replace("}", "}}");
		};
	}

	/// <summary>Adds the plugin bridging Carbon's native hooks to Oxide's covalence hooks.</summary>
	private static void InstallCore()
	{
		if (ModLoader.FindPlugin(nameof(OxideCore)) != null)
		{
			return;
		}

		Community.Runtime.HookManager.LoadHooksFromType(typeof(CovalenceHooks));

		if (ModLoader.InitializePlugin(typeof(OxideCore), out var plugin, Community.Runtime.Core.Package, precompiled: true))
		{
			plugin.IsCorePlugin = true;
		}
	}

	/// <summary>Remaps precompiled Oxide extensions (carbon/extensions) onto Carbon and this package.</summary>
	private sealed class OxideExtensionConverter : AssemblyConverter
	{
		private readonly BaseConverter _converter = new OxideConverter();

		public override string Name => "Oxide extension";

		public override bool Handles(AddonType type) => type == AddonType.Extension;

		public override ConversionResult Convert(string file, AddonType type, ref byte[] raw)
		{
			var module = ModuleDefinition.FromBytes(raw, CompatManager.readerArgs);

			if (!module.AssemblyReferences.Any(OxideHelpers.IsOxideASM))
			{
				return ConversionResult.Skip;
			}

			return CompatManager.ConvertAssembly(module, _converter, ref raw) ? ConversionResult.Success : ConversionResult.Fail;
		}
	}
}
