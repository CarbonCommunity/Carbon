using API.Logger;
using Command = API.Commands.Command;

namespace Carbon.Core;

[Serializable]
public class Config
{
	public bool DeveloperMode { get; set; } = false;
	public bool IsModded { get; set; } = true;
	public List<Command.Prefix> Prefixes { get; set; } = new();
	public Dictionary<string, string> Aliases { get; set; }
	public bool Rcon { get; set; } = true;
	public string Language { get; set; } = "en";
	public string WebRequestIp { get; set; }

	public WatchersConfig Watchers { get; set; } = new();
	public PermissionsConfig Permissions { get; set; } = new();
	public AnalyticsConfig Analytics { get; set; } = new();
	public SelfUpdatingConfig SelfUpdating { get; set; } = new();
	public DebuggingConfig Debugging { get; set; } = new();
	public ProcessorsConfig Processors { get; set; } = new();
	public PublicizerConfig Publicizer { get; set; } = new();
	public LoggingConfig Logging { get; set; } = new();
	public ProfilerConfig Profiler { get; set; } = new();
	public CompilerConfig Compiler { get; set; } = new();
	public MiscConfig Misc { get; set; } = new();

	internal readonly string[] _invalidAliases =
	[
		"c.",
		"carbon."
	];

	public bool IsValidAlias(string input, out string reason)
	{
		reason = default;

		if (input.Contains(" "))
		{
			return false;
		}

		foreach (var alias in _invalidAliases)
		{
			if (!input.StartsWith(alias, StringComparison.OrdinalIgnoreCase)) continue;
			reason = alias;
			return false;
		}

		return true;
	}

	public class SelfUpdatingConfig
	{
		public bool Enabled { get; set; } = true;
		public bool HookUpdates { get; set; } = true;
		public string RedirectUri { get; set; }
	}

	public class CompilerConfig
	{
		public bool UnloadOnFailure { get; set; } = false;
		public List<string> ConditionalCompilationSymbols { get; set; }
	}

	public class ProfilerConfig
	{
		public bool RecordingWarnings { get; set; } = true;
	}

	public class WatchersConfig
	{
		public bool ScriptWatchers { get; set; } = true;
		public bool ZipScriptWatchers { get; set; } = true;
		public SearchOption ScriptWatcherOption { get; set; } = SearchOption.TopDirectoryOnly;
	}

	public class PermissionsConfig
	{
		public string PlayerDefaultGroup { get; set; } = "default";
		public string AdminDefaultGroup { get; set; } = "admin";
		public string ModeratorDefaultGroup { get; set; } = "moderator";
		public bool AutoGrantPlayerGroup { get; set; } = true;
		public bool AutoGrantAdminGroup { get; set; } = true;
		public bool AutoGrantModeratorGroup { get; set; } = true;
		public bool BypassAdminCooldowns { get; set; } = false;
		public Permission.SerializationMode PermissionSerialization { get; set; } = Permission.SerializationMode.Protobuf;
		public bool SqlPermissionUserPreload { get; set; } = true;
	}

	public class ProcessorsConfig
	{
		public float ScriptProcessingRate = 0.2f;
		public float ZipScriptProcessingRate = 0.5f;
	}

	public class DebuggingConfig
	{
		public string ScriptDebuggingOrigin = string.Empty;
		public int HookLagSpikeThreshold = 1000;
	}

	public class LoggingConfig
	{
		public double LogSplitSize { get; set; } = 2.5;
		public Severity LogSeverity { get; set; } = Severity.Notice;
		public int LogFileMode { get; set; } = 2;
		public int LogVerbosity { get; set; } = 0;
		public bool CommandSuggestions { get; set; } = true;
	}

	public class AnalyticsConfig
	{
		public bool Enabled { get; set; } = true;
	}

	public class PublicizerConfig
	{
		public List<string> PublicizedAssemblies { get; set; }
		public List<string> PublicizerMemberIgnores { get; set; }
	}

	public class MiscConfig
	{
#if WIN
		public bool ShowConsoleInfo { get; set; } = true;
#endif
	}
}
