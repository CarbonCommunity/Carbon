///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

#if !(WIN || UNIX)
#error Target architecture not defined
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Core.Modules;
using Carbon.Core.Processors;
using Humanlights.Extensions;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Plugins;
using UnityEngine;

namespace Carbon.Core
{
	public class CarbonCore
	{
		public static string Version { get; set; } = "Unknown";
		public static string InformationalVersion { get; set; } = "Unknown";

		public static bool IsServerFullyInitialized => RelationshipManager.ServerInstance != null;
		public static CarbonCore Instance { get; set; }

		public static bool IsConfigReady => Instance != null && Instance.Config != null;

		public const OS OperatingSystem =
#if WIN
			 OS.Win;
#elif UNIX
			 OS.Linux;
#else
#error Target architecture not defined
#endif

		public enum OS
		{
			Win,
			Linux
		}

		public CarbonAddonProcessor Addon { get; set; }
		public CarbonConfig Config { get; set; }
		public RustPlugin CorePlugin { get; set; }
		public CarbonLoader.CarbonMod Plugins { get; set; }
		public Entities Entities { get; set; }
		public bool IsInitialized { get; set; }

		internal static List<string> _addons = new List<string> { "carbon." };

		public static bool IsAddon(string input)
		{
			input = input.ToLower().Trim();

			foreach (var addon in _addons)
			{
				if (input.Contains(addon)) return true;
			}

			return false;
		}

		#region Config

		public void LoadConfig()
		{
			if (!OsEx.File.Exists(GetConfigFile()))
			{
				SaveConfig();
				return;
			}

			Config = JsonConvert.DeserializeObject<CarbonConfig>(OsEx.File.ReadText(GetConfigFile()));
		}

		public void SaveConfig()
		{
			if (Config == null) Config = new CarbonConfig();

			OsEx.File.Create(GetConfigFile(), JsonConvert.SerializeObject(Config, Formatting.Indented));
		}

		#endregion

		#region Commands

		public List<OxideCommand> AllChatCommands { get; } = new List<OxideCommand>();
		public List<OxideCommand> AllConsoleCommands { get; } = new List<OxideCommand>();

		internal void _clearCommands(bool all = false)
		{
			if (all)
			{
				AllChatCommands.Clear();
				AllConsoleCommands.Clear();
			}
			else
			{
				AllChatCommands.RemoveAll(x => !(x.Plugin is IModule) && (x.Plugin is RustPlugin && !(x.Plugin as RustPlugin).IsCorePlugin));
				AllConsoleCommands.RemoveAll(x => !(x.Plugin is IModule) && (x.Plugin is RustPlugin && !(x.Plugin as RustPlugin).IsCorePlugin));
			}
		}
		internal void _installDefaultCommands()
		{
			CorePlugin = new CarbonCorePlugin { Name = "Core", IsCorePlugin = true };
			Plugins = new CarbonLoader.CarbonMod { Name = "Scripts", IsCoreMod = true };
			CorePlugin.IInit();

			CarbonLoader._loadedMods.Add(new CarbonLoader.CarbonMod { Name = "Carbon Community", IsCoreMod = true, Plugins = new List<RustPlugin> { CorePlugin } });
			CarbonLoader._loadedMods.Add(Plugins);

			CarbonLoader.ProcessCommands(typeof(CarbonCorePlugin), CorePlugin, prefix: "c");
		}

		#endregion

		#region Processors

		public CarbonProcessor CarbonProcessor { get; set; }
		public ScriptProcessor ScriptProcessor { get; set; }
		public WebScriptProcessor WebScriptProcessor { get; set; }
		public HarmonyProcessor HarmonyProcessor { get; set; }
		public ModuleProcessor ModuleProcessor { get; set; }

		internal void _installProcessors()
		{
			if (ScriptProcessor == null ||
				WebScriptProcessor == null ||
				HarmonyProcessor == null ||
				ModuleProcessor == null ||
				CarbonProcessor)
			{
				_uninstallProcessors();

				var gameObject = new GameObject("Processors");
				ScriptProcessor = gameObject.AddComponent<ScriptProcessor>();
				WebScriptProcessor = gameObject.AddComponent<WebScriptProcessor>();
				HarmonyProcessor = gameObject.AddComponent<HarmonyProcessor>();
				CarbonProcessor = gameObject.AddComponent<CarbonProcessor>();
				Addon = new CarbonAddonProcessor();
				ModuleProcessor = new ModuleProcessor();
				Entities = new Entities();
			}
			Carbon.Logger.Log("Installed processors");

			_registerProcessors();
		}
		internal void _registerProcessors()
		{
			if (ScriptProcessor != null) ScriptProcessor?.Start();
			if (WebScriptProcessor != null) WebScriptProcessor?.Start();
			if (HarmonyProcessor != null) HarmonyProcessor?.Start();

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
				if (HarmonyProcessor != null) HarmonyProcessor?.Dispose();
				if (ModuleProcessor != null) ModuleProcessor?.Dispose();
				if (CarbonProcessor != null) CarbonProcessor?.Dispose();
			}
			catch { }

			try
			{
				if (ScriptProcessor != null) UnityEngine.Object.DestroyImmediate(ScriptProcessor);
				if (WebScriptProcessor != null) UnityEngine.Object.DestroyImmediate(WebScriptProcessor);
				if (HarmonyProcessor != null) UnityEngine.Object.DestroyImmediate(HarmonyProcessor);
				if (CarbonProcessor != null) UnityEngine.Object.DestroyImmediate(CarbonProcessor);
			}
			catch { }

			try
			{
				if (obj != null) UnityEngine.Object.Destroy(obj);
			}
			catch { }
		}

		#endregion

		#region Paths

		public static string GetConfigFile()
		{
			return Path.Combine(GetRootFolder(), "config.json");
		}

		public static string GetRootFolder()
		{
			var folder = Path.GetFullPath(Path.Combine($"{Application.dataPath}/..", "carbon"));
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetConfigsFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "configs");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetModulesFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "modules");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetDataFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "data");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetPluginsFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "plugins");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetHarmonyFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "harmony");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetLogsFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "logs");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetLangFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "lang");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetTempFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "temp");
			Directory.CreateDirectory(folder);

			return folder;
		}
		public static string GetReportsFolder()
		{
			var folder = Path.Combine($"{GetRootFolder()}", "reports");
			Directory.CreateDirectory(folder);

			return folder;
		}

		#endregion

		#region Logging

		public static void LogCommand(object message, BasePlayer player = null)
		{
			if (player == null)
			{
				Carbon.Logger.Log(message);
				return;
			}

			player.SendConsoleCommand($"echo {message}");
		}

		#endregion

		#region Path

		public const string Name =
#if WIN
			"Carbon";
#elif UNIX
			"Carbon-Unix";
#endif
		public const string DllName =
#if WIN
			"Carbon.dll";
#elif UNIX
			"Carbon-Unix.dll";
#endif
		public static string DllPath => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HarmonyMods",
#if WIN
			"Carbon.dll"
#elif UNIX
			"Carbon-Unix.dll"
#endif
	));

		#endregion

		public static void ReloadPlugins()
		{
			CarbonLoader.LoadCarbonMods();
			ScriptLoader.LoadAll();
		}
		public static void ClearPlugins()
		{
			Instance?._clearCommands();
			CarbonLoader.UnloadCarbonMods();
		}

		public void RefreshConsoleInfo()
		{
#if WIN
			if (!IsServerFullyInitialized) return;
			if (ServerConsole.Instance.input.statusText.Length != 4) ServerConsole.Instance.input.statusText = new string[4];

			var version =
#if DEBUG
				InformationalVersion;
#else
				Version;
#endif

			ServerConsole.Instance.input.statusText[3] = $" Carbon v{version}, {CarbonLoader._loadedMods.Count:n0} mods, {CarbonLoader._loadedMods.Sum(x => x.Plugins.Count):n0} plgs";
#endif
		}

		public void Init()
		{
			if (IsInitialized) return;

			#region Handle Versions

			var assembly = typeof(CarbonCore).Assembly;

			try { InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; } catch { }
			try { Version = assembly.GetName().Version.ToString(); } catch { }

			#endregion

			LoadConfig();
			Carbon.Logger.Log("Loaded config");

			Carbon.Logger.Log($"Loading...");

			GetRootFolder();
			GetConfigsFolder();
			GetModulesFolder();
			GetDataFolder();
			GetPluginsFolder();
			GetHarmonyFolder();
			GetLogsFolder();
			GetLangFolder();
			GetReportsFolder();
			OsEx.Folder.DeleteContents(GetTempFolder());
			Carbon.Logger.Log("Loaded folders");

			_installProcessors();

			Interface.Initialize();

			_clearCommands();
			_installDefaultCommands();

			CarbonHookValidator.Refresh();
			Carbon.Logger.Log("Fetched oxide hooks");

			ReloadPlugins();

			Carbon.Logger.Log($"Loaded.");

			RefreshConsoleInfo();

			IsInitialized = true;

			Entities.Init();
		}
		public void UnInit()
		{
			_uninstallProcessors();
			_clearCommands(all: true);

			ClearPlugins();
			CarbonLoader._loadedMods.Clear();
			UnityEngine.Debug.Log($"Unloaded Carbon.");

#if WIN
			try
			{
				if (ServerConsole.Instance != null && ServerConsole.Instance.input != null)
				{
					ServerConsole.Instance.input.statusText[3] = "";
					ServerConsole.Instance.input.statusText = new string[3];
				}
			}
			catch { }
#endif

			Entities.Dispose();
		}
	}
}
