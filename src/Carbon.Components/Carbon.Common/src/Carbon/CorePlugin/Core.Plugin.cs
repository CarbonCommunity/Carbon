using Carbon.Events;
using Facepunch;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	/// <summary>A plugin unit found on disk, and the source responsible for it.</summary>
	public struct ProcessableFile
	{
		public string Id;
		public string Path;
		public PluginSource Source;

		public bool IsValid => Source != null && !string.IsNullOrEmpty(Id);
	}

	public static List<ProcessableFile> ProcessableFiles { get; } = [];

	/// <summary>Refreshes <see cref="ProcessableFiles"/> with every plugin unit present on disk.</summary>
	public static void ProcessableFilesLookup()
	{
		ProcessableFiles.Clear();

		foreach (var source in Community.Runtime.PluginSources.All)
		{
			foreach (var file in source.Discover())
			{
				if (source.IsBlacklisted(file))
				{
					continue;
				}

				ProcessableFiles.Add(new ProcessableFile { Id = source.GetKey(file), Path = file, Source = source });
			}
		}
	}

	public static ProcessableFile GetPluginFile(string shortName)
	{
		ProcessableFilesLookup();

		foreach (var file in ProcessableFiles)
		{
			if (!file.Id.Equals(shortName, StringComparison.InvariantCultureIgnoreCase))
			{
				continue;
			}
			return file;
		}

		return default;
	}

	public override bool IInit()
	{
		_defaultLogTrace = Application.GetStackTraceLogType(LogType.Log);
		_defaultWarningTrace = Application.GetStackTraceLogType(LogType.Warning);
		_defaultErrorTrace = Application.GetStackTraceLogType(LogType.Error);
		_defaultAssertTrace = Application.GetStackTraceLogType(LogType.Assert);
		_defaultExceptionTrace = Application.GetStackTraceLogType(LogType.Exception);

		ApplyStacktrace();

		HookableType = GetType();
		Hooks = new();

		foreach (var method in HookableType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			if (Community.Runtime.HookManager.IsHook(method.Name))
			{
				Community.Runtime.HookManager.Subscribe(method.Name, Name);

				var hash = HookStringPool.GetOrAdd(method.Name);
				if (!Hooks.Contains(hash)) Hooks.Add(hash);
			}
		}

		if (!base.IInit())
		{
			return false;
		}

		foreach (var player in BasePlayer.activePlayerList)
		{
			permission.RefreshUser(player);
		}

		timer.Every(5f, () =>
		{
			if (Community.Runtime == null || Logger.CoreLog == null || !Logger.CoreLog.HasInit || Logger.CoreLog.PendingCount == 0 || Community.Runtime.Config.Logging.LogFileMode != 1)
			{
				return;
			}

			Logger.CoreLog.Flush();
		});

		cmd.AddConsoleCommand("help", this, nameof(Help), authLevel: 2, help: "HELP!");
		cmd.AddConsoleCommand("harmony.mods", this, nameof(HarmonyMods), authLevel: 2, help: "Prints a full list of all active HarmonyMods processed by Rust.");
		cmd.AddConsoleCommand("sayas", this, nameof(SayAs), authLevel: 2, help: "Sends a message in chat. It's basically `global.say` but customizable.");

		return true;
	}

	private void OnServerInitialized()
	{
		Community.Runtime.Modules.OnServerInit();
		CommandLine.ExecuteCommands("+carbon.onserverinit", "OnServerInitialized");

		var serverConfigPath = Path.Combine(ConVar.Server.GetServerFolder("cfg"), "server.cfg");
		var lines = OsEx.File.Exists(serverConfigPath) ? OsEx.File.ReadTextLines(serverConfigPath) : null;

		if (lines != null)
		{
			CommandLine.ExecuteCommands("+carbon.onserverinit", "cfg/server.cfg", lines);
			Array.Clear(lines, 0, lines.Length);
		}

		WebControlPanel.ServerInit();
	}

	private void OnServerSave()
	{
		Community.Runtime.Permission.SaveData();
		Community.Runtime.Modules.OnServerSave();

		Community.Runtime.Events
			.Trigger(CarbonEvent.OnServerSave, EventArgs.Empty);

#if !MINIMAL
		CarbonAuto.Singleton?.Save();
#endif
	}

	internal static StackTraceLogType _defaultLogTrace;
	internal static StackTraceLogType _defaultWarningTrace;
	internal static StackTraceLogType _defaultErrorTrace;
	internal static StackTraceLogType _defaultAssertTrace;
	internal static StackTraceLogType _defaultExceptionTrace;

	public static void ApplyStacktrace()
	{
		Application.SetStackTraceLogType(LogType.Log, _defaultLogTrace);
		Application.SetStackTraceLogType(LogType.Warning, _defaultWarningTrace);
		Application.SetStackTraceLogType(LogType.Error, _defaultErrorTrace);
		Application.SetStackTraceLogType(LogType.Assert, _defaultAssertTrace);
		Application.SetStackTraceLogType(LogType.Exception, _defaultExceptionTrace);
	}

	protected override void LoadDefaultMessages()
	{
		lang.RegisterMessages(Localisation.Phrases, this);
	}
}
