using System;
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
		GetRootFolder();
		GetConfigsFolder();
		GetModulesFolder();
		GetDataFolder();
		GetScriptFolder();
		GetExtensionsFolder();
		GetLogsFolder();
		GetLangFolder();
		GetReportsFolder();
		OsEx.Folder.DeleteContents(GetTempFolder());
		Logger.Log("Loaded folders");
	}

	#region Paths

	internal static string _customRootFolder;
	internal static string _customScriptFolder;
	internal static string _customConfigFolder;
	internal static string _customDataFolder;
	internal static string _customModuleFolder;
	internal static string _customExtensionsFolder;
	internal static bool _commandLineInitialized;

	internal static void _initializeCommandLine()
	{
		if (_commandLineInitialized) return;
		_commandLineInitialized = true;

		_customRootFolder = CommandLineEx.GetArgumentResult("-carbon.rootdir");
		_customScriptFolder = CommandLineEx.GetArgumentResult("-carbon.scriptdir");
		_customConfigFolder = CommandLineEx.GetArgumentResult("-carbon.configdir");
		_customDataFolder = CommandLineEx.GetArgumentResult("-carbon.datadir");
		_customModuleFolder = CommandLineEx.GetArgumentResult("-carbon.moduledir");
		_customExtensionsFolder = CommandLineEx.GetArgumentResult("-carbon.extdir");
	}

	public static string GetConfigFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetRootFolder(), "config.json");
	}

	public static string GetRootFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customRootFolder) ? Path.Combine($"{Application.dataPath}/..", "carbon") : _customRootFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetConfigsFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customConfigFolder) ? Path.Combine(GetRootFolder(), "configs") : _customConfigFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetModulesFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customModuleFolder) ? Path.Combine(GetRootFolder(), "modules") : _customModuleFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetDataFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customDataFolder) ? Path.Combine(GetRootFolder(), "data") : _customDataFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetScriptFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customScriptFolder) ? Path.Combine(GetRootFolder(), "plugins") : _customScriptFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetExtensionsFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customExtensionsFolder) ? Path.Combine(GetRootFolder(), "extensions") : _customExtensionsFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetManagedFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine($"{GetRootFolder()}", "managed");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLogsFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine($"{GetRootFolder()}", "logs");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLangFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine($"{GetRootFolder()}", "lang");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetTempFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine($"{GetRootFolder()}", "temp");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetReportsFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine($"{GetRootFolder()}", "reports");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetRustRootFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath)));

		return folder;
	}
	public static string GetRustManagedFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, "Managed")));

		return folder;
	}

	#endregion
}
