namespace Carbon.Core;

[Serializable]
public class Defines
{
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

		try
		{
			OsEx.Folder.DeleteContents(GetTempFolder());
		}
		catch (Exception ex)
		{
			Logger.Warn($"Failed clearing up the temporary folder. ({ex.Message})\n{ex.StackTrace}");
		}

		Logger.Log("Loaded folders");
	}

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

		_customRootFolder = Switches.GetRootDir();
		_customCarbonConfigFolder = Switches.GetCarbonConfigDir();
		_customScriptFolder = Switches.GetScriptDir();
		_customConfigFolder = Switches.GetConfigDir();
		_customDataFolder = Switches.GetDataDir();
		_customModifierFolder = Switches.GetModifierDir();
		_customLangFolder = Switches.GetLangDir();
		_customModuleFolder = Switches.GetModuleDir();
		_customExtensionsFolder = Switches.GetExtDir();
		_customLogsFolder = Switches.GetLogDir();
		_customProfilesFolder = Switches.GetProfileDir();
	}

	public static string GetConfigFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetRootFolder(), "config.json");
	}
	public static string GetMonoProfilerConfigFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetRootFolder(), "config.profiler.json");
	}
	public static string GetCarbonAutoFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetRootFolder(), "config.auto.json");
	}
	public static string GetVaultFile()
	{
		return Path.Combine(GetRustIdentityFolder(), "carbon.vault");
	}
	public static string GetWebRconConfigFile()
	{
		_initializeCommandLine();
		return Path.Combine(GetRootFolder(), "config.webrcon.json");
	}

	public static string GetRootFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(string.IsNullOrEmpty(_customRootFolder) ? Path.Combine($"{Application.dataPath}/..", "carbon") : _customRootFolder);
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
		var folder = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, "..")));

		return folder;
	}
	public static string GetRustManagedFolder()
	{
		_initializeCommandLine();
		var folder = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, "Managed")));

		return folder;
	}
	public static string GetRustIdentityFolder()
	{
		return Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, "..", "server", ConVar.Server.identity)));
	}
}
