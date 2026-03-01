using System;
using System.IO;
using System.Reflection;
using API.Events;
using Components;
using Patches;
using Utility;

namespace Carbon;

public sealed class Bootstrap
{
	private static readonly string identifier;
	private static readonly string assemblyName;
	private static UnityEngine.GameObject _gameObject;
	private static HarmonyLib.Harmony _harmonyInstance;

	public static string Name =>  assemblyName;

	internal static HarmonyLib.Harmony Harmony => _harmonyInstance;

	internal static AnalyticsManager Analytics;

	internal static AssemblyManager AssemblyEx;

	internal static CommandManager Commands;

	internal static DownloadManager Downloader;

	internal static EventManager Events;

	internal static FileWatcherManager Watcher;

	static Bootstrap()
	{
		Carbon.Components.ConVarSnapshots.TakeSnapshot();

		identifier = $"{Guid.NewGuid():N}";
		assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
	}

	public static void Initialize()
	{
		Utility.Logger.Log($"{assemblyName} loaded.");
		_harmonyInstance = new HarmonyLib.Harmony(identifier);

		var logPath = Path.Combine(Context.CarbonLogs, "Carbon.Harmony.log");

		Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", logPath);
		typeof(HarmonyLib.FileLog).GetField("_logPathInited", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
#if DEBUG
		HarmonyLib.Harmony.DEBUG = true;
#elif RELEASE
		HarmonyLib.Harmony.DEBUG = false;
#endif

		if(File.Exists(logPath))
		{
			File.Delete(logPath);
		}

		_gameObject = new UnityEngine.GameObject("Carbon");
		UnityEngine.Object.DontDestroyOnLoad(_gameObject);

		// top priority
		Commands = _gameObject.AddComponent<CommandManager>();
		Events = _gameObject.AddComponent<EventManager>();
		Watcher = _gameObject.AddComponent<FileWatcherManager>();

		// standard priority
		Analytics = _gameObject.AddComponent<AnalyticsManager>();
		Downloader = _gameObject.AddComponent<DownloadManager>();

		Events.Subscribe(CarbonEvent.StartupShared, x =>
		{
			AssemblyEx = _gameObject.AddComponent<AssemblyManager>();
			AssemblyEx.Components.Load("Carbon.dll", "CarbonEvent.StartupShared");
		});

		Events.Subscribe(CarbonEvent.CarbonStartupComplete, x =>
		{
			Watcher.enabled = true;
		});

		try
		{
			Utility.Logger.Log("Applying Harmony patches");
			Harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		catch (Exception e)
		{
			Utility.Logger.Error("Unable to apply all patches", e);
		}

		Events.Subscribe(CarbonEvent.HooksInstalled, x =>
		{
			FileSystem_WarmupHalt.IsReady = true;
		});
	}
}
