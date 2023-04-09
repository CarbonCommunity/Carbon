using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using API.Events;
using API.Hooks;
using Carbon.Base.Interfaces;
using Carbon.Components;
using Carbon.Extensions;
using Carbon.Plugins;
using ConVar;
using Facepunch;
using Facepunch.Utility;
using Network;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Libraries;
using Oxide.Plugins;
using UnityEngine;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;
using Pool = Facepunch.Pool;

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

	public static void RefreshOrderedFiles()
	{
		OrderedFiles.Clear();

		foreach (var file in OsEx.Folder.GetFilesWithExtension(Defines.GetScriptFolder(), "cs", SearchOption.TopDirectoryOnly))
		{
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
		ApplyStacktrace();

		Type = GetType();
		Hooks = new();

		foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
			{
				Community.Runtime.HookManager.Subscribe(method.Name, Name);

				var priority = method.GetCustomAttribute<HookPriority>();
				if (!Hooks.ContainsKey(method.Name)) Hooks.Add(method.Name, priority == null ? Priorities.Normal : priority.Priority);
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
	}

	private void OnServerInitialized()
	{
		Community.Runtime.ModuleProcessor.OnServerInit();
		CommandLine.ExecuteCommands("+carbon.onserverinit", "OnServerInitialized");

		var serverConfigPath = Path.Combine(ConVar.Server.GetServerFolder("cfg"), "server.cfg");
		var lines = OsEx.File.Exists(serverConfigPath) ? OsEx.File.ReadTextLines(serverConfigPath) : null; if (lines != null)
		{
			CommandLine.ExecuteCommands("+carbon.onserverinit", "cfg/server.cfg", lines);
			Array.Clear(lines, 0, lines.Length);
			lines = null;
		}
	}

	private void OnPlayerDisconnected(BasePlayer player, string reason)
	{
		Logger.Log($"{player.net.connection} left: {reason}");
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

	public static void Reply(object message, ConsoleSystem.Arg arg)
	{
		if (arg != null && arg.Player() != null)
		{
			arg.Player().SendConsoleCommand($"echo {message}");
			return;
		}

		if (message is string) arg.ReplyWith(message.ToString());
		else arg.ReplyWith(message);
	}

	internal static StackTraceLogType _defaultLogTrace = Application.GetStackTraceLogType(LogType.Log);
	internal static StackTraceLogType _defaultWarningTrace = Application.GetStackTraceLogType(LogType.Warning);
	internal static StackTraceLogType _defaultErrorTrace = Application.GetStackTraceLogType(LogType.Error);
	internal static StackTraceLogType _defaultAssertTrace = Application.GetStackTraceLogType(LogType.Assert);
	internal static StackTraceLogType _defaultExceptionTrace = Application.GetStackTraceLogType(LogType.Exception);

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
