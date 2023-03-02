using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using API.Contracts;
using API.Events;
using Carbon.Base.Interfaces;
using Carbon.Contracts;
using Carbon.Core;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Plugins;
using UnityEngine;

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

	public IAnalyticsManager Analytics { get; }
	public IDownloadManager Downloader { get; }
	public IEventManager Events { get; }

	public IHookManager HookManager { get; set; }
	public IScriptProcessor ScriptProcessor { get; set; }
	public IModuleProcessor ModuleProcessor { get; set; }
	public IWebScriptProcessor WebScriptProcessor { get; set; }
	public ICarbonProcessor CarbonProcessor { get; set; }

	public static bool IsServerFullyInitialized => IsServerFullyInitializedCache = RelationshipManager.ServerInstance != null;
	public static bool IsServerFullyInitializedCache { get; internal set; }

	public static bool IsConfigReady => Runtime != null && Runtime.Config != null;

	public Config Config { get; set; }
	public RustPlugin CorePlugin { get; set; }
	public Loader.CarbonMod Plugins { get; set; }
	public Entities Entities { get; set; }

	public Community()
	{
		try
		{
			GameObject gameObject = GameObject.Find("Carbon")
				?? throw new Exception("Carbon GameObject not found");

			Analytics = gameObject.GetComponent<IAnalyticsManager>();
			Downloader = gameObject.GetComponent<IDownloadManager>();
			Events = gameObject.GetComponent<IEventManager>();

			Events.Subscribe(CarbonEvent.StartupSharedComplete, args =>
			{
				IAnalyticsManager Identity = gameObject.GetComponent<IAnalyticsManager>();
				Logger.Log($"Carbon fingerprint: {Identity.ClientID}");
				Analytics.StartSession();
			});

			Events.Subscribe(CarbonEvent.AllPluginsLoaded, args =>
			{
				string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
				{
					true => "windows",
					false => "linux"
				};

				string branch = InformationalVersion switch
				{
					string s when s.Contains("Debug") => "debug",
					string s when s.Contains("Staging") => "staging",
					string s when s.Contains("Release") => "release",
					_ => "Unknown"
				};

				Analytics.LogEvent("on_server_initialized", new Dictionary<string, object>
				{
					{ "branch", branch },
					{ "platform", platform },
					{ "short_version", Version },
					{ "full_version", InformationalVersion },
					{ "plugin_count", Loader.LoadedMods.Sum(x => x.Plugins.Count) },
				});
			});

			Events.Subscribe(CarbonEvent.OnServerSave, args =>
			{
				Analytics.LogEvent("user_engagement", new Dictionary<string, object>
				{
					{ "engagement_time_msec", 0 }
				});
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Critical error", ex);
		}
	}

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
