using System;
using System.Reflection;
using Carbon;
using Carbon.Core;
using Carbon.Oxide;
using Oxide.Core.Libraries;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core;

public class OxideMod
{
	public DataFileSystem DataFileSystem { get; private set; } = new DataFileSystem(Defines.GetDataFolder());
	public PluginManager RootPluginManager { get; private set; }

	public Permission Permission { get; private set; }

	public string RootDirectory { get; private set; }
	public string InstanceDirectory { get; private set; }
	public string PluginDirectory { get; private set; }
	public string ConfigDirectory { get; private set; }
	public string DataDirectory { get; private set; }
	public string LangDirectory { get; private set; }
	public string LogDirectory { get; private set; }
	public string TempDirectory { get; private set; }

	public bool IsShuttingDown { get; private set; }

	internal static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

	public void Load()
	{
		InstanceDirectory = Defines.GetRootFolder();
		RootDirectory = Environment.CurrentDirectory;
		if (RootDirectory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
			RootDirectory = AppDomain.CurrentDomain.BaseDirectory;

		ConfigDirectory = Defines.GetConfigsFolder();
		DataDirectory = Defines.GetDataFolder();
		LangDirectory = Defines.GetLangFolder();
		LogDirectory = Defines.GetLogsFolder();
		PluginDirectory = Defines.GetScriptFolder();
		TempDirectory = Defines.GetTempFolder();

		DataFileSystem = new DataFileSystem(DataDirectory);
		RootPluginManager = new PluginManager();

		switch(Community.Runtime.Config.PermissionSerialization)
		{
			case Permission.SerializationMode.Protobuf:
				Permission = new Permission();
				break;

			case Permission.SerializationMode.SQL:
				Permission = new PermissionSql();
				break;
		}
	}

	public void NextTick(Action callback)
	{
		Community.Runtime.CarbonProcessor.OnFrameQueue.Enqueue(callback);
	}

	public void NextFrame(Action callback)
	{
		Community.Runtime.CarbonProcessor.OnFrameQueue.Enqueue(callback);
	}

	public void ReloadPlugin(string name)
	{
		var path = CorePlugin.GetPluginPath(name);

		Community.Runtime.HarmonyProcessor.Prepare(name, path);
		Community.Runtime.ScriptProcessor.Prepare(name, path);
	}

	public void UnloadPlugin(string name)
	{
		Community.Runtime.HarmonyProcessor.Remove(name);
		Community.Runtime.ScriptProcessor.Remove(name);
	}

	public void OnSave()
	{

	}

	public void OnShutdown()
	{
		if (!IsShuttingDown)
		{
			IsShuttingDown = true;
		}
	}

	public object CallHook(string hookName, params object[] args)
	{
		return HookCaller.CallStaticHook(hookName, args);
	}

	public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
	{
		return HookCaller.CallStaticDeprecatedHook(oldHook, newHook, expireDate, args);
	}

	public T GetLibrary<T>(string name = null) where T : Library
	{
		var type = typeof(T);

		if (type == typeof(Permission)) return Community.Runtime.CorePlugin.permission as T;
		else if (type == typeof(Lang)) return Community.Runtime.CorePlugin.lang as T;
		else if (type == typeof(Game.Rust.Libraries.Command)) return Community.Runtime.CorePlugin.cmd as T;
		else if (type == typeof(Game.Rust.Libraries.Rust)) return Community.Runtime.CorePlugin.rust as T;

		return Activator.CreateInstance<T>();
	}

	public static readonly VersionNumber Version = new(AssemblyVersion.Major, AssemblyVersion.Minor, AssemblyVersion.Build);

	#region Logging

	/// <summary>
	/// Outputs to the game's console a message with severity level 'NOTICE'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogInfo(string message)
		=> Logger.Log(message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogWarning(string message)
		=> Logger.Warn(message);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="ex"></param>
	public void LogError(string message, Exception ex)
		=> Logger.Error(message, ex);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	public void LogError(string message)
		=> Logger.Error(message, null);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="ex"></param>
	public void LogException(string message, Exception ex)
		=> Logger.Error(message, ex);

	/// <summary>
	/// Outputs to the game's console a message with severity level 'WARNING'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void PrintWarning(string format, params object[] args)
		=> Logger.Warn(string.Format(format, args));

	/// <summary>
	/// Outputs to the game's console a message with severity level 'ERROR'.
	/// NOTE: Oxide compatibility layer.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="args"></param>
	public void PrintError(string format, params object[] args)
		=> Logger.Error(string.Format(format, args));

	#endregion
}
