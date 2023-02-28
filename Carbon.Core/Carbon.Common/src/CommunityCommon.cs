using System.Collections.Generic;
using System.Linq;
using API.Contracts;
using Carbon.Base.Interfaces;
using Carbon.Extensions;
using Carbon.Core;
using Newtonsoft.Json;
using Oxide.Plugins;
using Carbon.Contracts;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

public class Community
{
	public static Community Runtime { get; set; }

	public static string Version { get; set; } = "Unknown";
	public static string InformationalVersion { get; set; } = "Unknown";

	public IDownloadManager Downloader { get; set; }
	public IEventManager Events { get; set; }

	public ICarbonProcessor CarbonProcessor { get; set; }
	public IScriptProcessor ScriptProcessor { get; set; }
	public IWebScriptProcessor WebScriptProcessor { get; set; }
	public IModuleProcessor ModuleProcessor { get; set; }
	public IHookManager HookManager { get; set; }

	public static bool IsServerFullyInitialized => IsServerFullyInitializedCache = RelationshipManager.ServerInstance != null;
	public static bool IsServerFullyInitializedCache { get; internal set; }

	public static bool IsConfigReady => Runtime != null && Runtime.Config != null;

	public Config Config { get; set; }
	public RustPlugin CorePlugin { get; set; }
	public Loader.CarbonMod Plugins { get; set; }
	public Entities Entities { get; set; }

	public void ClearCommands(bool all = false)
	{
		if (all)
		{
			AllChatCommands.Clear();
			AllConsoleCommands.Clear();
		}
		else
		{
			AllChatCommands.RemoveAll(x => !(x.Plugin is IModule) && (x.Plugin is RustPlugin && !(x.Plugin as RustPlugin).IsCorePlugin));
			AllConsoleCommands.RemoveAll(x => !(x.Plugin is IModule) && (x.Plugin is RustPlugin && !(x.Plugin as RustPlugin).IsCorePlugin));
		}
	}

	#region Config

	public void LoadConfig()
	{
		if (!OsEx.File.Exists(Defines.GetConfigFile()))
		{
			SaveConfig();
			return;
		}

		Config = JsonConvert.DeserializeObject<Config>(OsEx.File.ReadText(Defines.GetConfigFile()));

		var needsSave = false;
		if (Config.ConditionalCompilationSymbols == null)
		{
			Config.ConditionalCompilationSymbols = new();
			needsSave = true;
		}

		if (!Config.ConditionalCompilationSymbols.Contains("CARBON"))
			Config.ConditionalCompilationSymbols.Add("CARBON");

		Config.ConditionalCompilationSymbols = Config.ConditionalCompilationSymbols.Distinct().ToList();

		if (needsSave) SaveConfig();
	}

	public void SaveConfig()
	{
		if (Config == null) Config = new Config();

		OsEx.File.Create(Defines.GetConfigFile(), JsonConvert.SerializeObject(Config, Formatting.Indented));
	}

	#endregion

	#region Plugins

	public virtual void ReloadPlugins()
	{
		Loader.ClearAllErrored();
		Loader.ClearAllRequirees();
	}
	public void ClearPlugins()
	{
		Runtime.ClearCommands();
		Loader.UnloadCarbonMods();
	}

	#endregion

	public void RefreshConsoleInfo()
	{
#if WIN
		if (!IsConfigReady || !Config.ShowConsoleInfo) return;

		if (!IsServerFullyInitialized) return;
		if (ServerConsole.Instance.input.statusText.Length != 4) ServerConsole.Instance.input.statusText = new string[4];

		var version =
#if DEBUG
			InformationalVersion;
#else
            Version;
#endif

		ServerConsole.Instance.input.statusText[3] = $" Carbon v{version}, {Loader.LoadedMods.Count:n0} mods, {Loader.LoadedMods.Sum(x => x.Plugins.Count):n0} plgs";
#endif
	}

	#region Commands

	public List<OxideCommand> AllChatCommands { get; } = new List<OxideCommand>();
	public List<OxideCommand> AllConsoleCommands { get; } = new List<OxideCommand>();

	#endregion

	#region Logging

	public static void LogCommand(object message, BasePlayer player = null)
	{
		if (player == null)
		{
			Carbon.Logger.Log(message);
			return;
		}

		player.SendConsoleCommand($"echo {message}");
	}

	#endregion
}
