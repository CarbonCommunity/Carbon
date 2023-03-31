using System;
using System.IO;
using System.Reflection;
using API.Events;
using Components;
using Contracts;
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

	internal static AssemblyManagerEx AssemblyEx
	{ get => _gameObject.GetComponent<AssemblyManagerEx>(); }

	internal static DownloadManager Downloader
	{ get => _gameObject.GetComponent<DownloadManager>(); }

	internal static EventManager Events
	{ get => _gameObject.GetComponent<EventManager>(); }

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

#if DEBUG
		HarmonyLib.Harmony.DEBUG = true;
		Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", Path.Combine(Context.CarbonLogs, "harmony.log"));
		typeof(HarmonyLib.FileLog).GetField("_logPathInited", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
#endif

		_gameObject = new UnityEngine.GameObject("Carbon");
		UnityEngine.Object.DontDestroyOnLoad(_gameObject);

		_gameObject.AddComponent<AnalyticsManager>();
		_gameObject.AddComponent<AssemblyManagerEx>();
		_gameObject.AddComponent<DownloadManager>();
		_gameObject.AddComponent<EventManager>();
		_gameObject.AddComponent<FileWatcherManager>();

		Events.Subscribe(CarbonEvent.StartupShared, x =>
		{
			AssemblyEx.LoadComponent("Carbon.dll", "CarbonEvent.StartupShared");
		});

		Events.Subscribe(CarbonEvent.CarbonStartupComplete, x =>
		{
			Watcher.enabled = true;
			AssemblyEx.LoadModule("Carbon.Modules.dll", "CarbonEvent.CarbonStartupComplete");
		});

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
