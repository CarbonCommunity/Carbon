using System;
using System.Collections.Generic;
using System.IO;
using API.Events;
using Carbon.Components;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Hooks;
using Carbon.Processors;
using Oxide.Core;
using Oxide.Plugins;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

#if !(WIN || UNIX)
#error Target architecture not defined
#endif

public class CommunityInternal : Community
{
	public static CommunityInternal InternalRuntime { get { return Runtime as CommunityInternal; } set { Runtime = value; } }

	public bool IsInitialized { get; set; }

	public override void ReloadPlugins()
	{
		base.ReloadPlugins();

		ScriptLoader.LoadAll();
	}

	internal void _installDefaultCommands()
	{
		CorePlugin = new CorePlugin { Name = "Core", IsCorePlugin = true };
		Plugins = new Loader.CarbonMod { Name = "Scripts", IsCoreMod = false };
		CorePlugin.IInit();

		Loader.LoadedMods.Add(new Loader.CarbonMod { Name = "Carbon Community", IsCoreMod = true, Plugins = new List<RustPlugin> { CorePlugin } });
		Loader.LoadedMods.Add(Plugins);

		Loader.ProcessCommands(typeof(CorePlugin), CorePlugin, prefix: "c");
		Loader.ProcessCommands(typeof(CorePlugin), CorePlugin, prefix: "carbon");
	}

	#region Processors


	internal void _installProcessors()
	{
		Carbon.Logger.Log("Installed processors");
		{
			_uninstallProcessors();

			var gameObject = new GameObject("Processors");
			ScriptProcessor = gameObject.AddComponent<ScriptProcessor>();
			WebScriptProcessor = gameObject.AddComponent<WebScriptProcessor>();
			CarbonProcessor = gameObject.AddComponent<CarbonProcessor>();
			CommandManager = gameObject.AddComponent<CommandManager>();
			HookManager = gameObject.AddComponent<PatchManager>();
			ModuleProcessor = new ModuleProcessor();
			Entities = new Entities();
		}

		_registerProcessors();
	}
	internal void _registerProcessors()
	{
		if (ScriptProcessor != null) ScriptProcessor?.Start();
		if (WebScriptProcessor != null) WebScriptProcessor?.Start();

		if (ScriptProcessor != null) ScriptProcessor.InvokeRepeating(() => { RefreshConsoleInfo(); }, 1f, 1f);
		Carbon.Logger.Log("Registered processors");
	}
	internal void _uninstallProcessors()
	{
		var obj = ScriptProcessor == null ? null : ScriptProcessor.gameObject;

		try
		{
			if (ScriptProcessor != null) ScriptProcessor?.Dispose();
			if (WebScriptProcessor != null) WebScriptProcessor?.Dispose();
			if (ModuleProcessor != null) ModuleProcessor?.Dispose();
			if (CarbonProcessor != null) CarbonProcessor?.Dispose();
		}
		catch { }

		try
		{
			if (obj != null) UnityEngine.Object.Destroy(obj);
		}
		catch { }
	}

	#endregion

	public void Initialize()
	{
		if (IsInitialized) return;

		HookCaller.Caller = new HookCallerInternal();

		Events.Trigger(CarbonEvent.CarbonStartup, EventArgs.Empty);

		LoadConfig();
		Carbon.Logger.Log("Loaded config");

		Events.Subscribe(CarbonEvent.HookValidatorRefreshed, args =>
		{
			ClearCommands();
			_installDefaultCommands();
			ModuleProcessor.Init();

			CommandLine.ExecuteCommands("+carbon.onboot", "Carbon boot");

			var serverConfigPath = Path.Combine(ConVar.Server.GetServerFolder("cfg"), "server.cfg");
			var lines = OsEx.File.Exists(serverConfigPath) ? OsEx.File.ReadTextLines(serverConfigPath) : null;
			if (lines != null)
			{
				CommandLine.ExecuteCommands("+carbon.onboot", "cfg/server.cfg", lines);
				Array.Clear(lines, 0, lines.Length);
				lines = null;
			}

			if (!ConVar.Global.skipAssetWarmup_crashes)
			{
				ReloadPlugins();
			}
		});
		if (ConVar.Global.skipAssetWarmup_crashes)
		{
			Events.Subscribe(CarbonEvent.OnServerInitialized, args =>
			{
				ReloadPlugins();
			});
		}

		Carbon.Logger.Log($"Loading...");
		{
			Defines.Initialize();
			HookValidator.Initialize();

			_installProcessors();

			Interface.Initialize();

			RefreshConsoleInfo();

			IsInitialized = true;
		}
		Carbon.Logger.Log($"Loaded.");
		Events.Trigger(CarbonEvent.CarbonStartupComplete, EventArgs.Empty);

		Entities.Init();
	}
	public void Uninitalize()
	{
		try
		{
			Events.Trigger(CarbonEvent.CarbonShutdown, EventArgs.Empty);

			_uninstallProcessors();
			ClearCommands(all: true);

			ClearPlugins();
			Loader.LoadedMods.Clear();
			UnityEngine.Debug.Log($"Unloaded Carbon.");

#if WIN
			try
			{
				if (IsConfigReady && Config.ShowConsoleInfo && ServerConsole.Instance != null && ServerConsole.Instance.input != null)
				{
					ServerConsole.Instance.input.statusText = new string[3];
				}
			}
			catch { }
#endif

			Entities.Dispose();

			Carbon.Logger.Dispose();
		}
		catch (Exception ex)
		{
			Carbon.Logger.Error($"Failed Carbon uninitialization.", ex);
			Events.Trigger(CarbonEvent.CarbonShutdownFailed, EventArgs.Empty);
		}
	}
}
