///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            null;
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
		public bool IsInitialized { get; set; }

		internal static List<string> _addons = new List<string> { "carbon." };

		public static bool IsAddon ( string input )
		{
			input = input.ToLower ().Trim ();

			foreach ( var addon in _addons )
			{
				if ( input.Contains ( addon ) ) return true;
			}

			return false;
		}

		#region Config

		public void LoadConfig ()
		{
			if ( !OsEx.File.Exists ( GetConfigFile () ) )
			{
				SaveConfig ();
				return;
			}

			Config = JsonConvert.DeserializeObject<CarbonConfig> ( OsEx.File.ReadText ( GetConfigFile () ) );
		}

		public void SaveConfig ()
		{
			if ( Config == null ) Config = new CarbonConfig ();

			OsEx.File.Create ( GetConfigFile (), JsonConvert.SerializeObject ( Config, Formatting.Indented ) );
		}

		#endregion

		#region Commands

		public List<OxideCommand> AllChatCommands { get; } = new List<OxideCommand> ();
		public List<OxideCommand> AllConsoleCommands { get; } = new List<OxideCommand> ();

		internal void _clearCommands ( bool all = false )
		{
			if ( all )
			{
				AllChatCommands.Clear ();
				AllConsoleCommands.Clear ();
			}
			else
			{
				AllChatCommands.RemoveAll ( x => !x.Plugin.IsCorePlugin );
				AllConsoleCommands.RemoveAll ( x => !x.Plugin.IsCorePlugin );
			}
		}
		internal void _installDefaultCommands ()
		{
			CorePlugin = new CarbonCorePlugin { Name = "Core", IsCorePlugin = true };
			Plugins = new CarbonLoader.CarbonMod { Name = "Scripts", IsCoreMod = true };
			CorePlugin.IInit ();

			CarbonLoader._loadedMods.Add ( new CarbonLoader.CarbonMod { Name = "Carbon Community", IsCoreMod = true, Plugins = new List<RustPlugin> { CorePlugin } } );
			CarbonLoader._loadedMods.Add ( Plugins );

			CarbonLoader.ProcessCommands ( typeof ( CarbonCorePlugin ), CorePlugin, prefix: "c" );
		}

		#endregion

		#region Processors

		public ScriptProcessor ScriptProcessor { get; set; }
		public WebScriptProcessor WebScriptProcessor { get; set; }
		public HarmonyProcessor HarmonyProcessor { get; set; }

		internal void _installProcessors ()
		{
			if ( ScriptProcessor == null ||
				WebScriptProcessor == null ||
				HarmonyProcessor == null )
			{
				_uninstallProcessors ();

				var gameObject = new GameObject ( "Processors" );
				ScriptProcessor = gameObject.AddComponent<ScriptProcessor> ();
				WebScriptProcessor = gameObject.AddComponent<WebScriptProcessor> ();
				HarmonyProcessor = gameObject.AddComponent<HarmonyProcessor> ();
				Addon = new CarbonAddonProcessor ();
			}
			Debug ( "Installed processors", 3 );

			_registerProcessors ();
		}
		internal void _registerProcessors ()
		{
			if ( ScriptProcessor != null ) ScriptProcessor?.Start ();
			if ( WebScriptProcessor != null ) WebScriptProcessor?.Start ();
			if ( HarmonyProcessor != null ) HarmonyProcessor?.Start ();

			if ( ScriptProcessor != null ) ScriptProcessor.InvokeRepeating ( () => { RefreshConsoleInfo (); }, 1f, 1f );
			Debug ( "Registered processors", 3 );
		}
		internal void _uninstallProcessors ()
		{
			var obj = ScriptProcessor == null ? null : ScriptProcessor.gameObject;

			try
			{
				if ( ScriptProcessor != null ) ScriptProcessor?.Dispose ();
				if ( WebScriptProcessor != null ) WebScriptProcessor?.Dispose ();
				if ( HarmonyProcessor != null ) HarmonyProcessor?.Dispose ();
			}
			catch { }

			try
			{
				if ( WebScriptProcessor != null ) UnityEngine.Object.DestroyImmediate ( WebScriptProcessor );
				if ( HarmonyProcessor != null ) UnityEngine.Object.DestroyImmediate ( HarmonyProcessor );
			}
			catch { }

			try
			{
				if ( obj != null ) UnityEngine.Object.Destroy ( obj );
			}
			catch { }
		}

		#endregion

		#region Paths

		public static string GetConfigFile ()
		{
			return Path.Combine ( GetRootFolder (), "config.json" );
		}

		public static string GetRootFolder ()
		{
			var folder = Path.GetFullPath ( Path.Combine ( $"{Application.dataPath}/..", "carbon" ) );
			Directory.CreateDirectory ( folder );

			return folder;
		}
		public static string GetConfigsFolder ()
		{
			var folder = Path.Combine ( $"{GetRootFolder ()}", "configs" );
			Directory.CreateDirectory ( folder );

			return folder;
		}
		public static string GetDataFolder ()
		{
			var folder = Path.Combine ( $"{GetRootFolder ()}", "data" );
			Directory.CreateDirectory ( folder );

			return folder;
		}
		public static string GetPluginsFolder ()
		{
			var folder = Path.Combine ( $"{GetRootFolder ()}", "plugins" );
			Directory.CreateDirectory ( folder );

			return folder;
		}
		public static string GetLogsFolder ()
		{
			var folder = Path.Combine ( $"{GetRootFolder ()}", "logs" );
			Directory.CreateDirectory ( folder );

			return folder;
		}
		public static string GetLangFolder ()
		{
			var folder = Path.Combine ( $"{GetRootFolder ()}", "lang" );
			Directory.CreateDirectory ( folder );

			return folder;
		}
		public static string GetTempFolder ()
		{
			var folder = Path.Combine ( $"{GetRootFolder ()}", "temp" );
			Directory.CreateDirectory ( folder );

			return folder;
		}

		#endregion

		#region Logging

		public static void Debug ( object message, int level = 0, LogType log = LogType.Log )
		{
			if ( Instance.Config.Debug <= -1 ||
				Instance.Config.Debug <= level ) return;

			switch ( log )
			{
				case LogType.Log:
					Log ( $"[Carbon] {message}" );
					break;

				case LogType.Warning:
					Warn ( $"[Carbon] {message}" );
					break;

				case LogType.Error:
					Error ( $"[Carbon] {message}" );
					break;
			}
		}
		public static void Debug ( object header, object message, int level = 0, LogType log = LogType.Log )
		{
			Debug ( $"[{header}] {message}", level, log );
		}

		public static void Log ( object message )
		{
			UnityEngine.Debug.Log ( $"{message}" );
		}
		public static void Warn ( object message )
		{
			UnityEngine.Debug.LogWarning ( $"{message}" );
		}
		public static void Error ( object message, Exception exception = null )
		{
			if ( exception == null ) UnityEngine.Debug.LogError ( message );
			else UnityEngine.Debug.LogError ( new Exception ( $"{message}\n{exception}" ) );
		}

		public static void Format ( string format, params object [] args )
		{
			Log ( string.Format ( format, args ) );
		}
        public static void LogCommand ( object message, BasePlayer player = null )
        {
            if ( player == null )
            {
                Log ( message );
                return;
            }

            player.SendConsoleCommand ( $"echo {message}" );
        }
        public static void WarnFormat ( string format, params object [] args )
		{
			Warn ( string.Format ( format, args ) );
		}
		public static void ErrorFormat ( string format, Exception exception = null, params object [] args )
		{
			Error ( string.Format ( format, args ), exception );
		}

		#endregion

		public static void ReloadPlugins ()
		{
			CarbonLoader.LoadCarbonMods ();
			ScriptLoader.LoadAll ();
		}
		public static void ClearPlugins ()
		{
			Instance?._clearCommands ();
			CarbonLoader.UnloadCarbonMods ();
		}

		public void RefreshConsoleInfo ()
		{
#if WIN
			if ( !IsServerFullyInitialized ) return;
			if ( ServerConsole.Instance.input.statusText.Length != 4 ) ServerConsole.Instance.input.statusText = new string [ 4 ];

			var version =
#if DEBUG
				InformationalVersion;
#else
				Version;
#endif

			ServerConsole.Instance.input.statusText [ 3 ] = $" Carbon v{version}, {CarbonLoader._loadedMods.Count:n0} mods, {CarbonLoader._loadedMods.Sum ( x => x.Plugins.Count ):n0} plgs";
#endif
		}

		public void Init ()
		{
			if ( IsInitialized ) return;

			#region Handle Versions

			var assembly = typeof ( CarbonCore ).Assembly;

            try { InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute> ().InformationalVersion; } catch { }
			try { Version = assembly.GetName().Version.ToString(); } catch { }

			#endregion

			LoadConfig ();
			Debug ( "Loaded config", 3 );

			Format ( $"Loading..." );

			GetRootFolder ();
			GetConfigsFolder ();
			GetDataFolder ();
			GetPluginsFolder ();
			GetLogsFolder ();
			GetLangFolder ();
			OsEx.Folder.DeleteContents ( GetTempFolder () );
			Debug ( "Loaded folders", 3 );

			_installProcessors ();

			Interface.Initialize ();

			_clearCommands ();
			_installDefaultCommands ();

			ReloadPlugins ();

			Format ( $"Loaded." );

			RefreshConsoleInfo ();

			IsInitialized = true;
		}
		public void UnInit ()
		{
			_uninstallProcessors ();
			_clearCommands ( all: true );

			ClearPlugins ();
			CarbonLoader._loadedMods.Clear ();
			UnityEngine.Debug.Log ( $"Unloaded Carbon." );

#if WIN
			try
			{
				if ( ServerConsole.Instance != null && ServerConsole.Instance.input != null )
				{
					ServerConsole.Instance.input.statusText [ 3 ] = "";
					ServerConsole.Instance.input.statusText = new string [ 3 ];
				}
			}
			catch { }
#endif
		}
	}

	public class CarbonInitializer : IHarmonyModHooks
	{
		public void OnLoaded ( OnHarmonyModLoadedArgs args )
		{
			var oldMod = PlayerPrefs.GetString ( Harmony_Load.CARBON_LOADED );

			if ( !Assembly.GetExecutingAssembly ().FullName.StartsWith ( oldMod ) )
			{
				CarbonCore.Instance?.UnInit ();
				HarmonyLoader.TryUnloadMod ( oldMod );
				CarbonCore.WarnFormat ( $"Unloaded previous: {oldMod}" );
				CarbonCore.Instance = null;
			}

			CarbonCore.Format ( "Initializing..." );

			if ( CarbonCore.Instance == null ) CarbonCore.Instance = new CarbonCore ();
			else CarbonCore.Instance?.UnInit ();

			CarbonCore.Instance.Init ();
		}

		public void OnUnloaded ( OnHarmonyModUnloadedArgs args ) { }
	}

	[Serializable]
	public class CarbonConfig
	{
		public int Debug { get; set; }

		public bool CarbonTag { get; set; } = true;
		public bool IsModded { get; set; } = true;
		public bool HookTimeTracker { get; set; } = false;
		public bool ScriptWatchers { get; set; } = true;
		public bool HarmonyWatchers { get; set; } = true;
	}
}