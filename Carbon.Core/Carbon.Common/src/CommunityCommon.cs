using System;
using System.Collections.Generic;
using System.Linq;
using API.Analytics;
using API.Assembly;
using API.Commands;
using API.Contracts;
using API.Events;
using API.Hooks;
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

	public static GameObject GameObject { get => _gameObject.Value; }
	private static readonly Lazy<GameObject> _gameObject = new(() =>
	{
		GameObject gameObject = GameObject.Find("Carbon");
		return gameObject == null ? throw new Exception("Carbon GameObject not found") : gameObject;
	});

	public IAnalyticsManager Analytics { get => _analyticsManager.Value; }
	private readonly Lazy<IAnalyticsManager> _analyticsManager
		= new(GameObject.GetComponent<IAnalyticsManager>);

	public IAssemblyManager AssemblyEx { get => _assemblyEx.Value; }
	private readonly Lazy<IAssemblyManager> _assemblyEx
		= new(GameObject.GetComponent<IAssemblyManager>);

	public IDownloadManager Downloader { get => _downloadManager.Value; }
	private readonly Lazy<IDownloadManager> _downloadManager
		= new(GameObject.GetComponent<IDownloadManager>);

	public IEventManager Events { get => _eventManager.Value; }
	private readonly Lazy<IEventManager> _eventManager
		= new(GameObject.GetComponent<IEventManager>);


	public IPatchManager HookManager { get; set; }
	public ICommandManager CommandManager { get; set; }
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
			Events.Subscribe(CarbonEvent.CarbonStartup, args =>
			{
				Logger.Log($"Carbon fingerprint: {Analytics.ClientID}");
				Analytics.SessionStart();
			});

			Events.Subscribe(CarbonEvent.CarbonStartupComplete, args =>
			{
				Analytics.LogEvent("on_server_startup",
					segments: new Dictionary<string, object> {
						{ "branch", Analytics.Branch },
						{ "platform", Analytics.Platform },
					},
					metrics: new Dictionary<string, object> {
						{ "version", Analytics.Version },
						{ "protocol", Analytics.Protocol },
					}
				);
			});

			Events.Subscribe(CarbonEvent.AllPluginsLoaded, args =>
			{
				Analytics.LogEvent("on_server_initialized",
					segments: new Dictionary<string, object> {
						{ "branch", Analytics.Branch },
						{ "platform", Analytics.Platform },
					},
					metrics: new Dictionary<string, object> {
						{ "plugin_count", Loader.LoadedMods.Sum(x => x.Plugins.Count) }
					}
				);
			});
		}
		catch (Exception ex)
		{
			Logger.Error("Critical error", ex);
		}
	}

	public void ClearCommands(bool all = false)
	{
		CommandManager.ClearCommands(command => all || command.Reference is RustPlugin plugin && !plugin.IsCorePlugin);
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

		if (!Config.ConditionalCompilationSymbols.Contains("RUST"))
			Config.ConditionalCompilationSymbols.Add("RUST");

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
		Loader.IsBatchComplete = false;
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
			Analytics.InformationalVersion;
#else
            Analytics.Version;
#endif

		ServerConsole.Instance.input.statusText[3] = $" Carbon v{version}, {Loader.LoadedMods.Count:n0} mods, {Loader.LoadedMods.Sum(x => x.Plugins.Count):n0} plgs";
#endif
	}

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
