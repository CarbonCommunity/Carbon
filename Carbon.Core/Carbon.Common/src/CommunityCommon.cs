using API.Analytics;
using API.Assembly;
using API.Commands;
using API.Contracts;
using API.Events;
using API.Hooks;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

public abstract class Community
{
	public static Community Runtime
	{ get; set; }

	public static GameObject GameObject
	{ get => _gameObject.Value; }

	private static readonly Lazy<GameObject> _gameObject = new(() =>
	{
		GameObject gameObject = GameObject.Find("Carbon");
		return gameObject == null ? throw new Exception("Carbon GameObject not found") : gameObject;
	});

	public IAnalyticsManager Analytics
	{ get => _analyticsManager.Value; }

	public IAssemblyManager AssemblyEx
	{ get => _assemblyEx.Value; }

	public ICommandManager CommandManager
	{ get => _commandManager.Value; }

	public IDownloadManager Downloader
	{ get => _downloadManager.Value; }

	public IEventManager Events
	{ get => _eventManager.Value; }

	private readonly Lazy<IAnalyticsManager> _analyticsManager
		= new(GameObject.GetComponent<IAnalyticsManager>);

	private readonly Lazy<IAssemblyManager> _assemblyEx
		= new(GameObject.GetComponent<IAssemblyManager>);

	private readonly Lazy<ICommandManager> _commandManager
		= new(GameObject.GetComponent<ICommandManager>);

	private readonly Lazy<IDownloadManager> _downloadManager
		= new(GameObject.GetComponent<IDownloadManager>);

	private readonly Lazy<IEventManager> _eventManager
		= new(GameObject.GetComponent<IEventManager>);

#if EXPERIMENTAL
	public IThreadManager Threads
	{ get => _threadManager.Value; }

	private readonly Lazy<IThreadManager> _threadManager
		= new(GameObject.GetComponent<IThreadManager>);
#endif

	public IPatchManager HookManager
	{ get; set; }

	public IScriptProcessor ScriptProcessor
	{ get; set; }

	public IModuleProcessor ModuleProcessor
	{ get; set; }

	public IWebScriptProcessor WebScriptProcessor
	{ get; set; }

	public ICarbonProcessor CarbonProcessor
	{ get; set; }

	public static bool IsServerFullyInitialized => IsServerFullyInitializedCache = RelationshipManager.ServerInstance != null;

	public static bool IsServerFullyInitializedCache
	{ get; internal set; }

	public static bool IsConfigReady => Runtime != null && Runtime.Config != null;

	public Config Config
	{ get; set; }

	public RustPlugin CorePlugin
	{ get; set; }

	public ModLoader.ModPackage Plugins
	{ get; set; }

	public Entities Entities
	{ get; set; }

	internal static int Tick = DateTime.UtcNow.Year + DateTime.UtcNow.Month + DateTime.UtcNow.Day + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute + DateTime.UtcNow.Second + DateTime.UtcNow.Month;

	public static string Protect(string name)
	{
		if (string.IsNullOrEmpty(name)) return string.Empty;

		var split = name.Split(' ');
		var command = split[0];
		var args = split.Skip(1).ToArray();
		var arguments = args.ToString(" ");

		Array.Clear(split, 0, split.Length);
		Array.Clear(args, 0, args.Length);

		return $"carbonprotecc_{RandomEx.GetRandomString(16, command + Tick.ToString(), command.Length + Tick)} {arguments}".TrimEnd();
	}

	public Community()
	{
		try
		{
			Events.Subscribe(CarbonEvent.CarbonStartup, args =>
			{
				Logger.Log($"Carbon fingerprint: {Analytics.ClientID}");
				Logger.Log($"System fingerprint: {Analytics.SystemID}");
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
						{ "plugin_count", ModLoader.LoadedPackages.Sum(x => x.Plugins.Count) }
					}
				);
			});

			var newlineSplit = new char[] { '\n' };

			Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
			{
				switch (type)
				{
					case LogType.Error:
					case LogType.Exception:
					case LogType.Assert:
						if (!string.IsNullOrEmpty(condition) &&
						(condition.StartsWith("Null") || condition.StartsWith("Index")))
						{
							var trace = stackTrace.Split(newlineSplit, StringSplitOptions.RemoveEmptyEntries);
							var resultTrace = string.Empty;

							for (int i = 0; i < trace.Length; i++)
							{
								var t = trace[i];
								if (string.IsNullOrEmpty(t)) continue;

								resultTrace += $"  at {t}\n";
							}

							Array.Clear(trace, 0, trace.Length);

							resultTrace = resultTrace.TrimEnd();
							Logger.Write(API.Logger.Severity.Error, $"Unhandled error occurred ({condition})\n{resultTrace}", nativeLog: false);
							Console.WriteLine(resultTrace);
						}
						break;
				}
			};
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
		ModLoader.IsBatchComplete = false;
		ModLoader.ClearAllErrored();
		ModLoader.ClearAllRequirees();
	}
	public void ClearPlugins()
	{
		Runtime.ClearCommands();
		ModLoader.UnloadCarbonMods();
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

		ServerConsole.Instance.input.statusText[3] = $" Carbon" +
#if MINIMAL
			$" Minimal" +
#endif
			$" v{version}, {ModLoader.LoadedPackages.Count:n0} mods, {ModLoader.LoadedPackages.Sum(x => x.Plugins.Count):n0} plgs";
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

	public abstract void Initialize();
	public virtual void Uninitialize()
	{
		Runtime = null;
	}
}
