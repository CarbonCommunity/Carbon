using API.Logger;
using Command = API.Commands.Command;

namespace Carbon.Core;

[Serializable]
public class Config
{
	public bool DeveloperMode = false;
	public bool IsModded = true;
	public List<Command.Prefix> Prefixes = new();
	public Dictionary<string, string> Aliases;
	public bool Rcon = true;
	public string Language = "en";
	public string WebRequestIp;

	public WatchersConfig Watchers = new();
	public PermissionsConfig Permissions = new();
	public AnalyticsConfig Analytics = new();
	public SelfUpdatingConfig SelfUpdating = new();
	public DebuggingConfig Debugging = new();
	public ProcessorsConfig Processors = new();
	public PublicizerConfig Publicizer = new();
	public LoggingConfig Logging = new();
	public ProfilerConfig Profiler = new();
	public CompilerConfig Compiler = new();
	public MiscConfig Misc = new();

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
		public bool Enabled = true;
		public bool HookUpdates = true;
		public string RedirectUri;
	}

	public class CompilerConfig
	{
		public bool UnloadOnFailure = false;
		public List<string> ConditionalCompilationSymbols;
	}

	public class ProfilerConfig
	{
		public bool RecordingWarnings = true;
	}

	public class WatchersConfig
	{
		public bool ScriptWatchers = true;
		public bool ZipScriptWatchers = true;
		public SearchOption ScriptWatcherOption = SearchOption.TopDirectoryOnly;
	}

	public class PermissionsConfig
	{
		public string PlayerDefaultGroup = "default";
		public string AdminDefaultGroup = "admin";
		public string ModeratorDefaultGroup = "moderator";
		public bool AutoGrantPlayerGroup = true;
		public bool AutoGrantAdminGroup = true;
		public bool AutoGrantModeratorGroup = true;
		public bool BypassAdminCooldowns = false;
		public Permission.SerializationMode PermissionSerialization = Permission.SerializationMode.Protobuf;
		public bool SqlPermissionUserPreload = true;
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
		public double LogSplitSize = 2.5;
		public Severity LogSeverity = Severity.Notice;
		public int LogFileMode = 2;
		public int LogVerbosity = 0;
		public bool CommandSuggestions = true;
		public bool ReducedLogging = true;
	}

	public class AnalyticsConfig
	{
		public bool Enabled = true;
	}

	public class PublicizerConfig
	{
		public List<string> PublicizedAssemblies;
		public List<string> PublicizerMemberIgnores;
	}

	public class MiscConfig
	{
#if WIN
		public bool ShowConsoleInfo = true;
#endif
	}
}
