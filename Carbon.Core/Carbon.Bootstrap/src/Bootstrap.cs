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

	internal static PermissionManager Permissions
	{ get => _gameObject.GetComponent<PermissionManager>(); }

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
		//_gameObject.AddComponent<PermissionManager>();


		// Permissions.Users.Insert("10", null, null);
		// Permissions.Users.Insert("20", "manel", null);
		// Permissions.Users.Insert("30", null, "pt");
		// Permissions.Users.Insert("40", "jose", "fr");

		// Permissions.Groups.Insert("test1", "This is test 1", 50, null);
		// Permissions.Groups.Insert("test2", "This is test 2", 10, null);
		// Permissions.Groups.Insert("test3", "This is test 3", 60, "test1");

		// Permissions.Groups.Insert("test4", "This is test 4", default, null);
		// Permissions.Groups.Remove("test4");

		// Permissions.Groups.AddGroupPermission("test1", "foo.bar");
		// Permissions.Groups.AddGroupPermission("test1", "bar.foo");

		// Permissions.Groups.AddGroupPermission("test2", "poop.foo");
		// Permissions.Groups.AddGroupPermission("test2", "poop.bar");
		// Permissions.Groups.AddGroupPermission("test2", "poop.oops");

		// Permissions.Groups.AddGroupPermission("test4", "poop.oops");

		// Permissions.Groups.Remove("test1");

		// Permissions.Groups.RemoveGroupPermission("test2", "poop.oops");


		// Permissions.Groups.AddGroupPermission("test2", "poop.dupe");
		// Permissions.Groups.AddGroupPermission("test2", "poop.dupe");

		// Permissions.Users.AddUserPermission("10", "foo.1");
		// Permissions.Users.AddUserPermission("10", "foo.1");
		// Permissions.Users.AddUserPermission("10", "foo.2");

		// Permissions.Users.AddUserPermission("20", "foo.3");
		// Permissions.Users.AddUserPermission("20", "foo.4");
		// Permissions.Users.AddUserPermission("20", "foo.5");
		// Permissions.Users.RemoveUserPermission("20", "foo.5");

		// Permissions.Users.AddUserPermission("30", "foo.3");
		// Permissions.Users.AddUserPermission("30", "foo.4");
		// Permissions.Users.AddUserPermission("30", "foo.5");
		// Permissions.Users.ResetPermissions("30");

		Events.Subscribe(CarbonEvent.StartupShared, x =>
		{
			AssemblyEx.LoadComponent("Carbon.dll", "CarbonEvent.StartupShared");
		});

		Events.Subscribe(CarbonEvent.CarbonStartupComplete, x =>
		{
			Watcher.enabled = true;
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
