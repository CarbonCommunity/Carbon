using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using API.Events;
using Carbon.Extensions;
using Carbon.Plugins;
using ConVar;
using Network;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Plugins;
using UnityEngine;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;

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

			OrderedFiles.Add(Path.GetFileNameWithoutExtension(file), file);
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

				var priority = method.GetCustomAttribute<HookPriority>();
				var hash = HookCallerCommon.StringPool.GetOrAdd(method.Name);
				if (!Hooks.ContainsKey(hash)) Hooks.Add(hash, priority == null ? Priorities.Normal : priority.Priority);
			}
		}

		base.IInit();

		foreach (var player in BasePlayer.activePlayerList)
		{
			permission.RefreshUser(player);
		}

		timer.Every(5f, () =>
		{
			if (!Logger._file._hasInit || Logger._file._buffer.Count == 0 || Community.Runtime.Config.LogFileMode != 1) return;
			Logger._file._flush();
		});

		cmd.AddConsoleCommand("help", this, nameof(Help), authLevel: 2);

		_originalMaxPlayers = ConVar.Server.maxplayers;
		ConVar.Server.maxplayers = 0;
	}
	public override object InternalCallHook(uint hook, object[] args)
	{
		var result = (object)null;
		try
		{
			switch (hook)
			{
				case 4026444072:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { AddConditional(arg0_0); }
						break;
					}
				case 815132798:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { CarbonLoadConfig(arg0_0); }
						break;
					}
				case 2590157530:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { CarbonSaveConfig(arg0_0); }
						break;
					}
				case 1438190503:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { ClearMarkers(arg0_0); }
						break;
					}
				case 2768612739:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Conditionals(arg0_0); }
						break;
					}
				case 739121130:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Find(arg0_0); }
						break;
					}
				case 3083923848:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { FindChat(arg0_0); }
						break;
					}
				case 2312305252:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Grant(arg0_0); }
						break;
					}
				case 930025435:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Group(arg0_0); }
						break;
					}
				case 2374729573:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Help(arg0_0); }
						break;
					}
				case 3611612159:
					{
						if (args[0] is BasePlayer arg0_0 && args[1] is DoorCloser arg1_0) { result = ICanPickupEntity(arg0_0, arg1_0); }
						break;
					}
				case 2404648208:
					{
						if (args[0] is BaseCombatEntity arg0_0 && args[1] is HitInfo arg1_0) { result = IOnBaseCombatEntityHurt(arg0_0, arg1_0); }
						break;
					}
				case 447466736:
					{
						if (args[0] is BasePlayer arg0_0 && args[1] is HitInfo arg1_0) { result = IOnBasePlayerAttacked(arg0_0, arg1_0); }
						break;
					}
				case 1334997006:
					{
						if (args[0] is BasePlayer arg0_0 && args[1] is HitInfo arg1_0) { result = IOnBasePlayerHurt(arg0_0, arg1_0); }
						break;
					}
				case 284015616:
					{
						if (args[0] is BaseNetworkable arg0_0 && args[1] is BaseNetworkable.SaveInfo arg1_0) { IOnEntitySaved(arg0_0, arg1_0); }
						break;
					}
				case 1448274911:
					{
						if (args[0] is Item arg0_0 && args[1] is float arg1_0) { result = IOnLoseCondition(arg0_0, arg1_0); }
						break;
					}
				case 6843826:
					{
						if (args[0] is BaseNpc arg0_0 && args[1] is BaseEntity arg1_0) { result = IOnNpcTarget(arg0_0, arg1_0); }
						break;
					}
				case 1154014332:
					{
						if (args[0] is Connection arg0_0 && args[1] is AuthResponse arg1_0) { IOnPlayerBanned(arg0_0, arg1_0); }
						break;
					}
				case 787516416:
					{
						if (args[0] is ulong arg0_0 && args[1] is string arg1_0 && args[2] is string arg2_0 && args[3] is Chat.ChatChannel arg3_0 && args[4] is BasePlayer arg4_0) { result = IOnPlayerChat(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0); }
						break;
					}
				case 2581265021:
					{
						if (args[0] is BasePlayer arg0_0 && args[1] is string arg1_0) { result = IOnPlayerCommand(arg0_0, arg1_0); }
						break;
					}
				case 3691992858:
					{
						if (args[0] is BasePlayer arg0_0) { IOnPlayerConnected(arg0_0); }
						break;
					}
				case 2834650998:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { result = IOnServerCommand(arg0_0); }
						break;
					}
				case 2994038319:
					{
						{ IOnServerShutdown(); }
						break;
					}
				case 2603852676:
					{
						if (args[0] is Connection arg0_0) { result = IOnUserApprove(arg0_0); }
						break;
					}
				case 646765377:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { LoadModuleConfig(arg0_0); }
						break;
					}
				case 1102288545:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { LoadPlugin(arg0_0); }
						break;
					}
				case 2947791118:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Modules(arg0_0); }
						break;
					}
				case 1280390023:
					{
						if (args[0] is Connection arg0_0) { OnClientAuth(arg0_0); }
						break;
					}
				case 1215643893:
					{
						if (args[0] is BaseCombatEntity arg0_0 && args[1] is HitInfo arg1_0) { OnEntityDeath(arg0_0, arg1_0); }
						break;
					}
				case 3950726597:
					{
						if (args[0] is BaseEntity arg0_0) { OnEntityKill(arg0_0); }
						break;
					}
				case 3757549339:
					{
						if (args[0] is BaseEntity arg0_0) { OnEntitySpawned(arg0_0); }
						break;
					}
				case 2449451640:
					{
						if (args[0] is BasePlayer arg0_0 && args[1] is string arg1_0) { OnPlayerDisconnected(arg0_0, arg1_0); }
						break;
					}
				case 713841014:
					{
						if (args[0] is BasePlayer arg0_0 && args[1] is string arg1_0) { OnPlayerKicked(arg0_0, arg1_0); }
						break;
					}
				case 1711034593:
					{
						if (args[0] is BasePlayer arg0_0) { result = OnPlayerRespawn(arg0_0); }
						break;
					}
				case 3404655920:
					{
						if (args[0] is BasePlayer arg0_0) { OnPlayerRespawned(arg0_0); }
						break;
					}
				case 3042820326:
					{
						if (args[0] is Connection arg0_0 && args[1] is string arg1_0 && args[2] is string arg2_0) { OnPlayerSetInfo(arg0_0, arg1_0, arg2_0); }
						break;
					}
				case 4143864509:
					{
						if (args[0] is Plugin arg0_0) { OnPluginLoaded(arg0_0); }
						break;
					}
				case 3843290135:
					{
						if (args[0] is Plugin arg0_0) { OnPluginUnloaded(arg0_0); }
						break;
					}
				case 1330569572:
					{
						OnServerInitialized(); break;
					}
				case 2032593992:
					{
						{ OnServerSave(); }
						break;
					}
				case 541418764:
					{
						if (args[0] is ulong arg0_0) { OnServerUserRemove(arg0_0); }
						break;
					}
				case 4207598011:
					{
						if (args[0] is ulong arg0_0 && args[1] is ServerUsers.UserGroup arg1_0 && args[2] is string arg2_0 && args[3] is string arg3_0 && args[4] is long arg4_0) { OnServerUserSet(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0); }
						break;
					}
				case 2274417763:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Plugins(arg0_0); }
						break;
					}
				case 2274439007:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { PluginsFailed(arg0_0); }
						break;
					}
				case 1658890215:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { PluginsUnloaded(arg0_0); }
						break;
					}
				case 1175597617:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { PluginWarns(arg0_0); }
						break;
					}
				case 1720368164:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Reload(arg0_0); }
						break;
					}
				case 74793642:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { ReloadConfig(arg0_0); }
						break;
					}
				case 3352136118:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { RemoveConditional(arg0_0); }
						break;
					}
				case 3116521:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Report(arg0_0); }
						break;
					}
				case 3153338129:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Revoke(arg0_0); }
						break;
					}
				case 467705630:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { SaveAllModules(arg0_0); }
						break;
					}
				case 1680578758:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { SaveModuleConfig(arg0_0); }
						break;
					}
				case 2689723201:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { SetModule(arg0_0); }
						break;
					}
				case 2970803623:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { Show(arg0_0); }
						break;
					}
				case 1126445065:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { UnloadPlugin(arg0_0); }
						break;
					}
				case 3375401029:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { UserGroup(arg0_0); }
						break;
					}
				case 1544271710:
					{
						if (args[0] is ConsoleSystem.Arg arg0_0) { WipeUI(arg0_0); }
						break;
					}
			}
		}
		catch (System.Exception ex)
		{
			var exception = ex.InnerException ?? ex;
			Carbon.Logger.Error($"Failed to call internal-hook '{Carbon.HookCallerCommon.StringPool.GetOrAdd(hook)}' on plugin '{Name} v{Version}'", exception);
		}
		return result;
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
	}

	private void OnPlayerDisconnected(BasePlayer player, string reason)
	{
		HookCaller.CallStaticHook("OnUserDisconnected", player?.AsIPlayer(), reason);
		Logger.Log($"{player.net.connection} left: {reason}");

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

	private void OnServerSave()
	{
		Logger.Debug($"Saving Carbon state..", 1);
		Interface.Oxide.Permission.SaveData();
		Community.Runtime.ModuleProcessor.OnServerSave();

		Community.Runtime.Events
			.Trigger(CarbonEvent.OnServerSave, EventArgs.Empty);
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
