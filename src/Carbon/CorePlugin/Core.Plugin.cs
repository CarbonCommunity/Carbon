using API.Events;
using Facepunch;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	public static Dictionary<string, string> OrderedFiles { get; } = new Dictionary<string, string>();

	public static void RefreshOrderedFiles()
	{
		OrderedFiles.Clear();

		var config = Community.Runtime.Config;
		var processor = Community.Runtime.ScriptProcessor;

		foreach (var file in OsEx.Folder.GetFilesWithExtension(Defines.GetScriptsFolder(), "cs", config.Watchers.ScriptWatcherOption))
		{
			if (processor.IsBlacklisted(file))
			{
				continue;
			}

			var id = Path.GetFileNameWithoutExtension(file);

			if (!OrderedFiles.ContainsKey(id))
			{
				OrderedFiles.Add(id, file);
			}
		}
	}

	public static KeyValuePair<string, string> GetPluginPath(string shortName)
	{
		RefreshOrderedFiles();

		foreach (var file in OrderedFiles.Where(file => file.Key.Equals(shortName, StringComparison.InvariantCultureIgnoreCase)))
		{
			return new KeyValuePair<string, string>(file.Key, file.Value);
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

		cmd.AddConsoleCommand("help", this, nameof(Help), authLevel: 2);

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
	private void OnEntitySpawned(BaseEntity entity)
	{
		Entities.AddMap(entity);
	}
	private void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
	{
		Entities.RemoveMap(entity);
	}
	private void OnEntityKill(BaseEntity entity)
	{
		Entities.RemoveMap(entity);
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
