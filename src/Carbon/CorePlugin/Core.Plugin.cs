using API.Events;
using Facepunch;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	public struct ProcessableFile
	{
		public string Id;
		public string Path;
		public Types Type;

		public IBaseProcessor GetProcessor()
		{
			switch (Type)
			{
				case Types.Script:
					return Community.Runtime.ScriptProcessor;
				case Types.CSZIP:
					return Community.Runtime.ZipScriptProcessor;
#if DEBUG
				case Types.CSZIP_Dev:
					return Community.Runtime.ZipDevScriptProcessor;
#endif
			}
			return null;
		}

		public enum Types
		{
			Script,
			CSZIP,
			CSZIP_Dev
		}
	}

	public static List<ProcessableFile> ProcessableFiles { get; } = [];

	public static void ProcessableFilesLookup()
	{
		ProcessableFiles.Clear();

		static bool IsBlacklisted(string path)
		{
			return Community.Runtime.ScriptProcessor.IsBlacklisted(path)
			       || Community.Runtime.ZipScriptProcessor.IsBlacklisted(path)
#if DEBUG
			       || Community.Runtime.ZipDevScriptProcessor.IsBlacklisted(path)
#endif
				;
		}

		var config = Community.Runtime.Config;

		foreach (var file in OsEx.Folder.GetFilesWithExtension(Defines.GetScriptsFolder(), "cs", config.Watchers.ScriptWatcherOption))
		{
			if (IsBlacklisted(file))
			{
				continue;
			}

			ProcessableFile processableFile = default;
			processableFile.Id = Path.GetFileNameWithoutExtension(file);
			processableFile.Path = file;
			processableFile.Type = ProcessableFile.Types.Script;
			ProcessableFiles.Add(processableFile);
		}
		foreach (var file in OsEx.Folder.GetFilesWithExtension(Defines.GetScriptsFolder(), "cszip", config.Watchers.ScriptWatcherOption))
		{
			if (IsBlacklisted(file))
			{
				continue;
			}

			ProcessableFile processableFile = default;
			processableFile.Id = Path.GetFileNameWithoutExtension(file);
			processableFile.Path = file;
			processableFile.Type = ProcessableFile.Types.CSZIP;
			ProcessableFiles.Add(processableFile);
		}
#if DEBUG
		var zipDevPlugins = Directory.GetDirectories(Defines.GetZipDevFolder(), "*", SearchOption.TopDirectoryOnly);
		foreach (var file in zipDevPlugins)
		{
			ProcessableFile processableFile = default;
			processableFile.Id = Path.GetFileNameWithoutExtension(file);
			processableFile.Path = file;
			processableFile.Type = ProcessableFile.Types.CSZIP_Dev;
			ProcessableFiles.Add(processableFile);
		}
#endif
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
			if (Community.Runtime == null || Logger.CoreLog == null || !Logger.CoreLog.HasInit || Logger.CoreLog._buffer.Count == 0 || Community.Runtime.Config.Logging.LogFileMode != 1)
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
		Community.Runtime.ModuleProcessor.OnServerInit();
		CommandLine.ExecuteCommands("+carbon.onserverinit", "OnServerInitialized");

		var serverConfigPath = Path.Combine(ConVar.Server.GetServerFolder("cfg"), "server.cfg");
		var lines = OsEx.File.Exists(serverConfigPath) ? OsEx.File.ReadTextLines(serverConfigPath) : null;

		if (lines != null)
		{
			CommandLine.ExecuteCommands("+carbon.onserverinit", "cfg/server.cfg", lines);
			Array.Clear(lines, 0, lines.Length);
		}

		foreach (var player in BasePlayer.allPlayerList)
		{
			try
			{
				if (player.IsNpc)
				{
					continue;
				}
				player.AsIPlayer();
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed getting IPlayer object for {player.displayName}[{player.UserIDString}]", ex);
			}
		}

		WebControlPanel.ServerInit();
	}
	private void OnServerSave()
	{
		Logger.Debug($"Saving Carbon state..", 1);
		Interface.Oxide.Permission.SaveData();
		Community.Runtime.ModuleProcessor.OnServerSave();

		Community.Runtime.Events
			.Trigger(CarbonEvent.OnServerSave, EventArgs.Empty);

#if !MINIMAL
		API.Abstracts.CarbonAuto.Singleton?.Save();
#endif
	}

	private void OnPluginLoaded(Plugin plugin)
	{
		var eventArg = Pool.Get<CarbonEventArgs>();
		eventArg.Init(plugin);
		Community.Runtime.Events.Trigger(CarbonEvent.PluginLoaded, eventArg);
		Pool.Free(ref eventArg);
	}
	private void OnPluginUnloaded(Plugin plugin)
	{
		var eventArg = Pool.Get<CarbonEventArgs>();
		eventArg.Init(plugin);
		Community.Runtime.Events.Trigger(CarbonEvent.PluginUnloaded, eventArg);
		Pool.Free(ref eventArg);
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
