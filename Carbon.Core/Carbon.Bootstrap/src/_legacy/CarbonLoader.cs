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

namespace Legacy;

internal sealed class Loader : Singleton<Loader>, IDisposable
{
	private static readonly string identifier;
	private static readonly string assemblyName;

	private HarmonyLib.Harmony _harmonyInstance;


	internal string Name
	{ get => assemblyName; }

	internal HarmonyLib.Harmony Harmony
	{ get => _harmonyInstance; }


	private static UnityEngine.GameObject _gameObject;

	internal AnalyticsManager Analytics
	{ get => _gameObject.GetComponent<AnalyticsManager>(); }

	internal AssemblyManagerEx AssemblyEx
	{ get => _gameObject.GetComponent<AssemblyManagerEx>(); }

	internal DownloadManager Downloader
	{ get => _gameObject.GetComponent<DownloadManager>(); }

	internal EventManager Events
	{ get => _gameObject.GetComponent<EventManager>(); }

	internal FileWatcherManager Watcher
	{ get => _gameObject.GetComponent<FileWatcherManager>(); }


	static Loader()
	{
		identifier = $"{Guid.NewGuid():N}";
		Logger.Warn($"Using '{identifier}' as runtime namespace");
		assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
	}

	internal Loader()
	{
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
			AssemblyEx.LoadComponent("Carbon.dll", this);
		});

		Events.Subscribe(CarbonEvent.CarbonStartupComplete, x =>
		{
			Watcher.enabled = true;
			AssemblyEx.LoadModule("Carbon.Modules.dll", this);
		});
	}

	internal void Initialize()
	{
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

	public void Dispose()
	{
		try
		{
			Harmony.UnpatchAll(identifier);
			Logger.Log("Removed all Harmony patches");
		}
		catch (Exception e)
		{
			Logger.Error("Unable to remove all patches", e);
		}

		_harmonyInstance = default;
		_gameObject = default;
	}
}
