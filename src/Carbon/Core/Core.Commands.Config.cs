namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("editconfig", "When ran by an admin client, the Carbon Admin module will open up a config editor.")]
	private void EditConfig(ConsoleSystem.Arg arg)
	{
		if (arg.Player() is not BasePlayer player)
		{
			arg.ReplyWith("Only admin clients can run this command");
			return;
		}

		var file = arg.GetString(0);
		if (!OsEx.File.Exists(file))
		{
			arg.ReplyWith($"File '{file}' does not exist");
			return;
		}

		AdminModule.Singleton.SetTab(player, AdminModule.ConfigEditor.Make(OsEx.File.ReadText(file), (_, _) =>
		{
			AdminModule.Singleton.SetTab(player, 0);
			AdminModule.Singleton.Close(player);
		}, (_, jobj) =>
		{
			OsEx.File.Create(file, jobj.ToString(Newtonsoft.Json.Formatting.Indented));
			AdminModule.Singleton.SetTab(player, 0);
			AdminModule.Singleton.Close(player);
		}, null, true));
	}

	[CommandVar("developermode", "Enables developer mode which grants a few features that are designed and used by the developers.")]
	[AuthLevel(2)]
	private bool DeveloperMode { get { return Community.Runtime.Config.DeveloperMode; } set { Community.Runtime.Config.DeveloperMode = value; Community.Runtime.SaveConfig(); } }

	[ConsoleCommand("loadconfig", "Loads Carbon config from file.")]
	[AuthLevel(2)]
	private void CarbonLoadConfig(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime == null) return;

		Community.Runtime.LoadConfig();

		arg.ReplyWith("Loaded Carbon config.");
	}

	[ConsoleCommand("saveconfig", "Saves Carbon config to file.")]
	[AuthLevel(2)]
	private void CarbonSaveConfig(ConsoleSystem.Arg arg)
	{
		if (Community.Runtime == null) return;

		Community.Runtime.SaveConfig();

		arg.ReplyWith("Saved Carbon config.");
	}

	[CommandVar("modding", "Mark this server as modded or not.")]
	[AuthLevel(2)]
	private bool Modding { get { return Community.Runtime.Config.IsModded; } set { Community.Runtime.Config.IsModded = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("scriptwatchers", "When disabled, you must load/unload plugins manually with `c.load` or `c.unload`.")]
	[AuthLevel(2)]
	private bool ScriptWatchers { get { return Community.Runtime.Config.Watchers.ScriptWatchers; } set { Community.Runtime.Config.Watchers.ScriptWatchers = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("scriptwatchersoption", "Indicates wether the script watcher (whenever enabled) listens to the 'carbon/plugins' folder only, or its subfolders. (0 = Top-only directories, 1 = All directories)")]
	[AuthLevel(2)]
	private int ScriptWatchersOption
	{
		get
		{
			return (int)Community.Runtime.Config.Watchers.ScriptWatcherOption;
		}
		set
		{
			Community.Runtime.Config.Watchers.ScriptWatcherOption = (SearchOption)value;
			Community.Runtime.ScriptProcessor.IncludeSubdirectories = value == (int)SearchOption.AllDirectories;
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("modulewatchers", "When disabled, modules only get loaded when the server boots.")]
	[AuthLevel(2)]
	private bool ModuleWatchers { get { return Community.Runtime.Config.Watchers.ModuleWatchers; } set { Community.Runtime.Config.Watchers.ModuleWatchers = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("extensionwatchers", "When disabled, extensions only get loaded when the server boots.")]
	[AuthLevel(2)]
	private bool ExtensionWatchers { get { return Community.Runtime.Config.Watchers.ExtensionWatchers; } set { Community.Runtime.Config.Watchers.ExtensionWatchers = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("debug", "The level of debug logging for Carbon. Helpful for very detailed logs in case things break. (Set it to -1 to disable debug logging.)")]
	[AuthLevel(2)]
	private int CarbonDebug { get { return Community.Runtime.Config.Logging.LogVerbosity; } set { Community.Runtime.Config.Logging.LogVerbosity = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("logfiletype", "The mode for writing the log to file. (0=disabled, 1=saves updates every 5 seconds, 2=saves immediately)")]
	[AuthLevel(2)]
	private int LogFileType { get { return Community.Runtime.Config.Logging.LogFileMode; } set { Community.Runtime.Config.Logging.LogFileMode = Mathf.Clamp(value, 0, 2); Community.Runtime.SaveConfig(); } }

	[CommandVar("language", "Server language used by the Language API.")]
	[AuthLevel(2)]
	private string Language { get { return Community.Runtime.Config.Language; } set { Community.Runtime.Config.Language = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("unloadonfailure", "Unload already loaded plugins when recompilation attempt fails. (Disabled by default)")]
	[AuthLevel(2)]
	private bool UnloadOnFailure { get { return Community.Runtime.Config.Compiler.UnloadOnFailure; } set { Community.Runtime.Config.Compiler.UnloadOnFailure = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("bypassadmincooldowns", "Bypasses the command cooldowns for admin-authed players.")]
	[AuthLevel(2)]
	private bool BypassAdminCooldowns { get { return Community.Runtime.Config.Permissions.BypassAdminCooldowns; } set { Community.Runtime.Config.Permissions.BypassAdminCooldowns = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("logsplitsize", "The size for each log (in megabytes) required for it to be split into separate chunks.")]
	[AuthLevel(2)]
	private double LogSplitSize { get { return Community.Runtime.Config.Logging.LogSplitSize; } set { Community.Runtime.Config.Logging.LogSplitSize = value; Community.Runtime.SaveConfig(); } }

	[CommandVar("scriptprocessrate", "The speed of detecting local file changes for items in the carbon/plugins directory.")]
	[AuthLevel(2)]
	private float ScriptProcessingRate
	{
		get => Community.Runtime.Config.Processors.ScriptProcessingRate;
		set
		{
			Community.Runtime.Config.Processors.ScriptProcessingRate = value;
			Community.Runtime.ScriptProcessor.RefreshRate();
			Community.Runtime.SaveConfig();
		}
	}

	[CommandVar("zipscriptprocessrate", "The speed of detecting local file changes for zipscript items in the carbon/plugins directory.")]
	[AuthLevel(2)]
	private float ZipScriptProcessingRate
	{
		get => Community.Runtime.Config.Processors.ZipScriptProcessingRate;
		set
		{
			Community.Runtime.Config.Processors.ZipScriptProcessingRate = value;
			Community.Runtime.ZipScriptProcessor.RefreshRate();
#if DEBUG
			Community.Runtime.ZipDevScriptProcessor.RefreshRate();
#endif
			Community.Runtime.SaveConfig();
		}
	}

#if WIN
	[CommandVar("consoleinfo", "Show the Windows-only Carbon information at the bottom of the console.")]
	[AuthLevel(2)]
	private bool ConsoleInfo
	{
		get { return Community.Runtime.Config.Misc.ShowConsoleInfo; }
		set
		{
			Community.Runtime.Config.Misc.ShowConsoleInfo = value;

			if (value)
			{
				Community.Runtime.RefreshConsoleInfo();
			}
			else
			{
				if (ServerConsole.Instance != null && ServerConsole.Instance.input != null)
				{
					ServerConsole.Instance.input.statusText = new string[3];
				}
			}
		}
	}
#endif

	[ConsoleCommand("assignalias", "Assigns a new command alias. (Eg. c.assignalias myalias c.reload)")]
	[AuthLevel(2)]
	private void AssignAlias(ConsoleSystem.Arg arg)
	{
		var alias = arg.GetString(0);
		var command = arg.GetString(1);

		if (string.IsNullOrEmpty(alias))
		{
			arg.ReplyWith("Alias cannot be null");
			return;
		}

		if (string.IsNullOrEmpty(command))
		{
			arg.ReplyWith("Alias command cannot be null");
			return;
		}

		if (alias.Equals(command, StringComparison.OrdinalIgnoreCase))
		{
			arg.ReplyWith("Don't be silly");
			return;
		}

		var warn = ConsoleSystem.Index.All.Any(x => x.FullName.Equals(alias, StringComparison.OrdinalIgnoreCase))
			? " (BEWARE! The alias you used is the name of an existent Rust command. Unassign this alias to make it accessible.)"
			: null;

		if (!Community.Runtime.Config.IsValidAlias(alias, out var reason))
		{
			arg.ReplyWith($"Invalid alias detected. Using '{reason}' is prohibited.");
			return;
		}

		if (Community.Runtime.Config.Aliases.TryGetValue(alias, out var existentCommand))
		{
			arg.ReplyWith($"Overriding alias '{alias}' -> {command}:\n Old: {existentCommand}{warn}");
			Community.Runtime.Config.Aliases[alias] = command;
			Community.Runtime.SaveConfig();
			return;
		}

		Community.Runtime.Config.Aliases[alias] = command;
		arg.ReplyWith($"Assigned alias '{alias}' -> {command}{warn}");
		Community.Runtime.SaveConfig();
	}

	[ConsoleCommand("unassignalias", "Unassigns a command alias. (Eg. c.unassignalias myalias)")]
	[AuthLevel(2)]
	private void UnassignAlias(ConsoleSystem.Arg arg)
	{
		var alias = arg.GetString(0);

		if (string.IsNullOrEmpty(alias))
		{
			arg.ReplyWith("Alias cannot be null");
			return;
		}

		if (!Community.Runtime.Config.Aliases.ContainsKey(alias))
		{
			arg.ReplyWith($"Alias '{alias}' does not exist");
			return;
		}

		Community.Runtime.Config.Aliases.Remove(alias);
		arg.ReplyWith($"Unassigned alias '{alias}'");
		Community.Runtime.SaveConfig();
	}

	[ConsoleCommand("aliases", "Prints the full list of aliases and respective redirected commands.")]
	[AuthLevel(2)]
	private void Aliases(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith($"Found {Community.Runtime.Config.Aliases.Count:n0} {Community.Runtime.Config.Aliases.Count.Plural("alias", "aliases")}:\n{Community.Runtime.Config.Aliases.Select(x => $" {x.Key} -> {x.Value}").ToString("\n")}");
	}
}
