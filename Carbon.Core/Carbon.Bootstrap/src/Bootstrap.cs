using System;
using System.IO;
using System.Reflection;
using API.Commands;
using API.Events;
using Components;
using Facepunch;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

public sealed class Bootstrap
{
	private static readonly string identifier;
	private static readonly string assemblyName;
	private static UnityEngine.GameObject _gameObject;

	private static HarmonyLib.Harmony _harmonyInstance;

	public static string Name
	{ get => assemblyName; }

	internal static HarmonyLib.Harmony Harmony
	{ get => _harmonyInstance; }


	internal static AnalyticsManager Analytics
	{ get => _gameObject.GetComponent<AnalyticsManager>(); }

	internal static AssemblyManager AssemblyEx
	{ get => _gameObject.GetComponent<AssemblyManager>(); }

	internal static CommandManager Commands
	{ get => _gameObject.GetComponent<CommandManager>(); }

	internal static DownloadManager Downloader
	{ get => _gameObject.GetComponent<DownloadManager>(); }

	internal static EventManager Events
	{ get => _gameObject.GetComponent<EventManager>(); }

	internal static PermissionManager Permissions
	{ get => _gameObject.GetComponent<PermissionManager>(); }

#if EXPERIMENTAL
	internal static ThreadManager Threads
	{ get => _gameObject.GetComponent<ThreadManager>(); }
#endif

	internal static FileWatcherManager Watcher
	{ get => _gameObject.GetComponent<FileWatcherManager>(); }


	static Bootstrap()
	{
		identifier = $"{Guid.NewGuid():N}";
		Logger.Warn($"Using '{identifier}' as runtime namespace");
		assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
	}

	public static void Initialize()
	{
		Logger.Log($"{assemblyName} loaded.");
		_harmonyInstance = new HarmonyLib.Harmony(identifier);

		var logPath = Path.Combine(Context.CarbonLogs, "harmony.log");

		Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", logPath);
		typeof(HarmonyLib.FileLog).GetField("_logPathInited", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
#if DEBUG
		HarmonyLib.Harmony.DEBUG = true;
#elif RELEASE
		HarmonyLib.Harmony.DEBUG = false;
#endif

		if(File.Exists(logPath)) File.Delete(logPath);

		_gameObject = new UnityEngine.GameObject("Carbon");
		UnityEngine.Object.DontDestroyOnLoad(_gameObject);

		// top priority
		_gameObject.AddComponent<CommandManager>();
		_gameObject.AddComponent<EventManager>();
		_gameObject.AddComponent<FileWatcherManager>();

		// standard priority
		_gameObject.AddComponent<AnalyticsManager>();
		_gameObject.AddComponent<AssemblyManager>();
		_gameObject.AddComponent<DownloadManager>();

#if EXPERIMENTAL
		_gameObject.AddComponent<ThreadManager>();

		Events.Subscribe(CarbonEvent.FileSystemWarmupComplete, x =>
		{
			Threads.enabled = true;
		});
#endif

		//_gameObject.AddComponent<PermissionManager>();
		// Test2 test2 = new Test2();
		// test2.DoStuff(Permissions);
		// ITestInterface foo = Test1.GetInstance();
		// foo.DoStuff();

		Events.Subscribe(CarbonEvent.StartupShared, x =>
		{
			AssemblyEx.Components.Load("Carbon.dll", "CarbonEvent.StartupShared");
		});

		Events.Subscribe(CarbonEvent.CarbonStartupComplete, x =>
		{
			Watcher.enabled = true;
		});


		Commands.RegisterCommand(new Command.RCon
		{
			Name = "c.build",
			Callback = (arg) => arg.ReplyWith(Analytics.InformationalVersion)
		}, out string _);
		Commands.RegisterCommand(new Command.RCon
		{
			Name = "carbon.build",
			Callback = (arg) => arg.ReplyWith(Analytics.InformationalVersion)
		}, out string _);

		var versionArg = new Action<Command.Args>((arg) =>
		{
			if (arg.IsServer)
			{
				arg.ReplyWith($"Carbon" +
#if MINIMAL
					$" Minimal" +
#endif
					$" {Analytics.Version}/{Analytics.Platform}/{Analytics.Protocol} on Rust {BuildInfo.Current.Build.Number}/{Rust.Protocol.printable}");
			}
			else
			{
				arg.ReplyWith($"Carbon" +
#if MINIMAL
					$" Minimal" +
#endif
					$" <color=#d14419>{Analytics.Version}/{Analytics.Platform}/{Analytics.Protocol}</color> on Rust <color=#d14419>{BuildInfo.Current.Build.Number}/{Rust.Protocol.printable}</color>.");
			
			}
		});

		Commands.RegisterCommand(new Command.RCon
		{
			Name = "c.version",
			Callback = versionArg
		}, out string _);
		Commands.RegisterCommand(new Command.RCon
		{
			Name = "carbon.version",
			Callback = (arg) => arg.ReplyWith($"Carbon v{Analytics.Version}")
		}, out string _);

		Commands.RegisterCommand(new Command.ClientConsole
		{
			Name = "c.version",
			Callback = versionArg
		}, out string _);
		Commands.RegisterCommand(new Command.ClientConsole
		{
			Name = "carbon.version",
			Callback = (arg) => arg.ReplyWith($"Carbon v{Analytics.Version}")
		}, out string _);

		Commands.RegisterCommand(new Command.RCon
		{
			Name = "c.protocol",
			Callback = (arg) => arg.ReplyWith(Analytics.Protocol)
		}, out string _);
		Commands.RegisterCommand(new Command.RCon
		{
			Name = "carbon.protocol",
			Callback = (arg) => arg.ReplyWith(Analytics.Protocol)
		}, out string _);

		try
		{
			Logger.Log("Applying Harmony patches");
			Harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		catch (Exception e)
		{
			Logger.Error("Unable to apply all patches", e);
		}
	}
}
