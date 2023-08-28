using API.Events;
using Carbon.Client;
using ConVar;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;
using Connection = Network.Connection;
using Timer = Oxide.Plugins.Timer;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;
#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	public static Dictionary<string, string> OrderedFiles { get; } = new Dictionary<string, string>();

	internal int _originalMaxPlayers = 0;

	public static void RefreshOrderedFiles()
	{
		OrderedFiles.Clear();

		var config = Community.Runtime.Config;
		var processor = Community.Runtime.ScriptProcessor;

		foreach (var file in OsEx.Folder.GetFilesWithExtension(Defines.GetScriptFolder(), "cs", config.ScriptWatcherOption))
		{
			if (processor.IsBlacklisted(file)) continue;

			var id = Path.GetFileNameWithoutExtension(file);
			if (!OrderedFiles.ContainsKey(id)) OrderedFiles.Add(id, file);
		}
	}

	public static string GetPluginPath(string shortName)
	{
		foreach (var file in OrderedFiles)
		{
			if (file.Key == shortName) return file.Value;
		}

		return null;
	}

	public override void IInit()
	{
		_defaultLogTrace = Application.GetStackTraceLogType(LogType.Log);
		_defaultWarningTrace = Application.GetStackTraceLogType(LogType.Warning);
		_defaultErrorTrace = Application.GetStackTraceLogType(LogType.Error);
		_defaultAssertTrace = Application.GetStackTraceLogType(LogType.Assert);
		_defaultExceptionTrace = Application.GetStackTraceLogType(LogType.Exception);

		ApplyStacktrace();

		Type = GetType();
		Hooks = new();

		foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
			{
				Community.Runtime.HookManager.Subscribe(method.Name, Name);

				var hash = HookStringPool.GetOrAdd(method.Name);
				if (!Hooks.Contains(hash)) Hooks.Add(hash);
			}
		}

		base.IInit();

		foreach (var player in BasePlayer.activePlayerList)
		{
			permission.RefreshUser(player);
		}

		timer.Every(5f, () =>
		{
			if (Community.Runtime == null || Logger.CoreLog == null || !Logger.CoreLog.HasInit || Logger.CoreLog._buffer.Count == 0 || Community.Runtime.Config.LogFileMode != 1) return;
			Logger.CoreLog.Flush();
		});

		cmd.AddConsoleCommand("help", this, nameof(Help), authLevel: 2);

		_originalMaxPlayers = ConVar.Server.maxplayers;
		ConVar.Server.maxplayers = 0;
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
			lines = null;
		}

		var pluginCheck = (Timer)null;

		if (!Community.Runtime.ScriptProcessor.AllPendingScriptsComplete())
		{
			Logger.Warn($"There still are pending plugins loading... Temporarily disallowing players from joining (enabled queue).");
		}

		pluginCheck = timer.Every(1f, () =>
		{
			if (Community.Runtime.ScriptProcessor.AllPendingScriptsComplete() &&
				Community.Runtime.ScriptProcessor.AllNonRequiresScriptsComplete() &&
				Community.Runtime.ScriptProcessor.AllExtensionsComplete())
			{
				if (ConVar.Server.maxplayers != _originalMaxPlayers)
				{
					Logger.Warn($"All plugins have been loaded. Changing maximum players back to {_originalMaxPlayers}.");
					ConVar.Server.maxplayers = _originalMaxPlayers;
				}

				pluginCheck.Destroy();
				pluginCheck = null;
			}
		});

		CarbonAuto.Init();
		API.Abstracts.CarbonAuto.Singleton.Load();
	}
	private void OnServerSave()
	{
		Logger.Debug($"Saving Carbon state..", 1);
		Interface.Oxide.Permission.SaveData();
		Community.Runtime.ModuleProcessor.OnServerSave();

		Community.Runtime.Events
			.Trigger(CarbonEvent.OnServerSave, EventArgs.Empty);

		API.Abstracts.CarbonAuto.Singleton?.Save();
	}

	private void OnPlayerDisconnected(BasePlayer player, string reason)
	{
		HookCaller.CallStaticHook(4253366379, player?.AsIPlayer(), reason);
	
		if (player.IsAdmin && !player.IsOnGround())
		{
			var newPosition = player.transform.position;

			if (UnityEngine.Physics.Raycast(newPosition, Vector3.down, out var hit, float.MaxValue, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				newPosition.y = hit.point.y;

				if (Vector3.Distance(player.transform.position, newPosition) > 3.5f)
				{
					player.SetServerFall(false);
					player.Teleport(newPosition);
					player.estimatedVelocity = Vector3.zero;
					NextFrame(() =>
					{
						if (player != null)
						{
							player.SetServerFall(true);
						}
					});
					Logger.Warn($"Moved admin player {player.net.connection} on the object underneath so it doesn't die from fall damage.");
				}
			}
		}

		CarbonClient.Dispose(CarbonClient.Get(player));
	}
	private void OnPluginLoaded(Plugin plugin)
	{
		Community.Runtime.Events.Trigger(CarbonEvent.PluginLoaded, new CarbonEventArgs(plugin));
	}
	private void OnPluginUnloaded(Plugin plugin)
	{
		Community.Runtime.Events.Trigger(CarbonEvent.PluginUnloaded, new CarbonEventArgs(plugin));
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
		if (Community.Runtime.Config.UnityStacktrace)
		{
			Application.SetStackTraceLogType(LogType.Log, _defaultLogTrace);
			Application.SetStackTraceLogType(LogType.Warning, _defaultWarningTrace);
			Application.SetStackTraceLogType(LogType.Error, _defaultErrorTrace);
			Application.SetStackTraceLogType(LogType.Assert, _defaultAssertTrace);
			Application.SetStackTraceLogType(LogType.Exception, _defaultExceptionTrace);
		}
		else
		{
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.None);
		}
	}
}
