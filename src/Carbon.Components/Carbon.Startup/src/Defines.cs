using System.IO;
using System;
using Doorstop.Utility;
using Doorstop;
using Carbon.Startup.Extensions;

namespace Carbon.Startup.Core;

[Serializable]
public class Defines
{
	internal static string root;
	public static void Initialize()
	{
		GetRootFolder();
		GetCarbonConfigFolder();
		GetConfigsFolder();
		GetModulesFolder();
		GetDataFolder();
		GetScriptsFolder();
		GetExtensionsFolder();
		GetLogsFolder();
		GetLangFolder();
	}

	internal static string _customRustRootFolder;
	internal static string _customRootFolder;
	internal static string _customCarbonConfigFolder;
	internal static string _customScriptFolder;
	internal static string _customConfigFolder;
	internal static string _customDataFolder;
	internal static string _customModifierFolder;
	internal static string _customLangFolder;
	internal static string _customModuleFolder;
	internal static string _customExtensionsFolder;
	internal static string _customProfilesFolder;
	internal static string _customLogsFolder;
	internal static bool _commandLineInitialized;

	internal static void _initializeCommandLine()
	{
		if (_commandLineInitialized) return;
		_commandLineInitialized = true;

		try
		{
			root = Path.GetFullPath(Path.Combine("."));
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		_customRustRootFolder = "-carbon.rustrootdir".GetArgumentResult();
		_customRootFolder = "-carbon.rootdir".GetArgumentResult();
		_customCarbonConfigFolder = "-carbon.carbonconfigdir".GetArgumentResult();
		_customScriptFolder = "-carbon.scriptdir".GetArgumentResult();
		_customConfigFolder = "-carbon.configdir".GetArgumentResult();
		_customDataFolder = "-carbon.datadir".GetArgumentResult();
		_customModifierFolder = "-carbon.modifierdir".GetArgumentResult();
		_customLangFolder = "-carbon.langdir".GetArgumentResult();
		_customModuleFolder = "-carbon.moduledir".GetArgumentResult();
		_customExtensionsFolder = "-carbon.extdir".GetArgumentResult();
		_customLogsFolder = "-carbon.logdir".GetArgumentResult();
		_customProfilesFolder = "-carbon.profiledir".GetArgumentResult();
	}

	public static string GetConfigFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetCarbonConfigFolder(), "config.json");
	}
	public static string GetMonoProfilerConfigFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetCarbonConfigFolder(), "config.profiler.json");
	}
	public static string GetCarbonAutoFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetCarbonConfigFolder(), "config.auto.json");
	}

	public static string GetRootFolder()
	{
		_initializeCommandLine();
		var folder = string.IsNullOrEmpty(_customRootFolder) ? Path.Combine(root, "carbon") : _customRootFolder;
		Directory.CreateDirectory(folder);

		return folder;
	}

	public static string GetCarbonConfigFolder()
	{
		_initializeCommandLine();
		var folder = string.IsNullOrEmpty(_customCarbonConfigFolder) ? GetRootFolder() : _customRootFolder;
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetCompilerFolder()
	{
		string folder = Path.Combine($"{GetRootFolder()}", "compiler");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLibFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customModuleFolder) ? Path.Combine(GetManagedFolder(), "lib") : _customModuleFolder);
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
	public static string GetManagedModulesFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(GetManagedFolder(), "modules"));
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
	public static string GetModifierFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customModifierFolder) ? Path.Combine(GetRootFolder(), "modifiers") : _customModifierFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetScriptsFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customScriptFolder) ? Path.Combine(GetRootFolder(), "plugins") : _customScriptFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetScriptBackupFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(GetScriptsFolder(), "backups"));
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetScriptDebugFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(GetScriptsFolder(), "debug"));
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetZipDevFolder()
	{
		var folder = Path.Combine(GetScriptsFolder(), "cszip_dev");
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
	public static string GetDeveloperFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(GetRootFolder(), "developer"));
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetDeveloperPatchedAssembliesFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(GetDeveloperFolder(), "patched_assemblies"));
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetManagedFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine(GetRootFolder(), "managed");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetHooksFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine(GetManagedFolder(), "hooks");
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLogsFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customLogsFolder) ? Path.Combine(GetRootFolder(), "logs") : _customLogsFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetProfilesFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customProfilesFolder) ? Path.Combine(GetRootFolder(), "profiles") : _customProfilesFolder);
		Directory.CreateDirectory(folder);

		return folder;
	}
	public static string GetLangFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customLangFolder) ? Path.Combine(GetRootFolder(), "lang") : _customLangFolder);
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
	public static string GetRustRootFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customRustRootFolder) ? root : _customRustRootFolder);

		return folder;
	}
	public static string GetRustManagedFolder()
	{
		_initializeCommandLine();
		var folder = Path.Combine(Path.Combine(GetRustRootFolder(), "RustDedicated_Data", "Managed"));

		return folder;
	}
}
