using System;
using System.Collections.Generic;
using System.IO;
using Carbon.Extensions;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;

[Serializable]
public class Defines
{
	public static void Initialize()
	{
		_initializeCommandLine();

		GetRootFolder();
		GetConfigsFolder();
		GetModulesFolder();
		GetDataFolder();
		GetScriptFolder();
		GetHarmonyFolder();
		GetLogsFolder();
		GetLangFolder();
		GetReportsFolder();
		OsEx.Folder.DeleteContents(GetTempFolder());
		Logger.Log("Loaded folders");
	}

	#region Paths

	internal static string _customRootFolder;
	internal static string _customScriptFolder;
	internal static string _customHarmonyFolder;
	internal static string _customConfigFolder;
	internal static string _customDataFolder;
	internal static string _customModuleFolder;

	internal static void _initializeCommandLine()
	{
		_customRootFolder = CommandLineEx.GetArgumentResult("-carbon.rootdir");
		_customScriptFolder = CommandLineEx.GetArgumentResult("-carbon.scriptdir");
		_customHarmonyFolder = CommandLineEx.GetArgumentResult("-carbon.harmonydir");
		_customConfigFolder = CommandLineEx.GetArgumentResult("-carbon.configdir");
		_customDataFolder = CommandLineEx.GetArgumentResult("-carbon.datadir");
		_customModuleFolder = CommandLineEx.GetArgumentResult("-carbon.moduledir");
	}

	public static string GetConfigFile()
	{
		return Path.Combine(GetRootFolder(), "config.json");
	}

	public static string GetRootFolder()
	{
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customRootFolder) ? Path.Combine($"{Application.dataPath}/..", "carbon") : _customRootFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetConfigsFolder()
	{
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customConfigFolder) ? Path.Combine(GetRootFolder(), "configs") : _customConfigFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetModulesFolder()
	{
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customModuleFolder) ? Path.Combine(GetRootFolder(), "modules") : _customModuleFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetDataFolder()
	{
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customDataFolder) ? Path.Combine(GetRootFolder(), "data") : _customDataFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetScriptFolder()
	{
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customScriptFolder) ? Path.Combine(GetRootFolder(), "plugins") : _customScriptFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetHarmonyFolder()
	{
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customHarmonyFolder) ? Path.Combine(GetRootFolder(), "harmony") : _customHarmonyFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetManagedFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "managed");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLogsFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "logs");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLangFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "lang");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetTempFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "temp");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetReportsFolder()
	{
		var folder = Path.Combine($"{GetRootFolder()}", "reports");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetRustRootFolder()
	{
		var folder = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath)));

		return folder;
	}
	public static string GetRustManagedFolder()
	{
		var folder = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, "Managed")));

		return folder;
	}

	#endregion
}
