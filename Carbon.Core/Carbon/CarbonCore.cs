using Carbon.Core.Processors;
using Humanlights.Extensions;
using Oxide.Core;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Carbon.Core
{
    public class CarbonCore
    {
        public static VersionNumber Version { get; } = new VersionNumber ( 1, 0, 160 );

        public static bool IsServerFullyInitialized => RelationshipManager.ServerInstance != null;
        public static CarbonCore Instance { get; set; }

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

        public List<OxideCommand> AllChatCommands { get; } = new List<OxideCommand> ();
        public List<OxideCommand> AllConsoleCommands { get; } = new List<OxideCommand> ();

        public ScriptProcessor ScriptProcessor { get; set; } = new ScriptProcessor ();
        public WebScriptProcessor WebScriptProcessor { get; set; } = new WebScriptProcessor ();
        public HarmonyProcessor HarmonyProcessor { get; set; } = new HarmonyProcessor ();

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

        public static void Log ( object message )
        {
            Debug.Log ( $"{message}" );
        }
        public static void Warn ( object message )
        {
            Debug.LogWarning ( $"{message}" );
        }
        public static void Error ( object message, Exception exception = null )
        {
            if ( exception == null ) Debug.LogError ( message );
            else Debug.LogError ( new Exception ( $"{message}\n{exception}" ) );
        }

        public static void Format ( string format, params object [] args )
        {
            Log ( string.Format ( format, args ) );
        }
        public static void WarnFormat ( string format, params object [] args )
        {
            Warn ( string.Format ( format, args ) );
        }
        public static void ErrorFormat ( string format, Exception exception = null, params object [] args )
        {
            Error ( string.Format ( format, args ), exception );
        }

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
            CorePlugin.Init ();

            CarbonLoader._loadedMods.Add ( new CarbonLoader.CarbonMod { Name = "Carbon Community", IsCoreMod = true, Plugins = new List<RustPlugin> { CorePlugin } } );
            CarbonLoader._loadedMods.Add ( Plugins );

            CarbonLoader.ProcessCommands ( typeof ( CarbonCorePlugin ), CorePlugin, prefix: "c" );
        }
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
            }

            _registerProcessors ();
        }
        internal void _registerProcessors ()
        {
            if ( ScriptProcessor != null ) ScriptProcessor?.Start ();
            if ( WebScriptProcessor != null ) WebScriptProcessor?.Start ();
            if ( HarmonyProcessor != null ) HarmonyProcessor?.Start ();

            if ( ScriptProcessor != null ) ScriptProcessor.InvokeRepeating ( () => { RefreshConsoleInfo (); }, 1f, 1f );
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

        public void RefreshConsoleInfo ()
        {
#if WIN
            if ( !IsServerFullyInitialized ) return;
            if ( ServerConsole.Instance.input.statusText.Length != 4 ) ServerConsole.Instance.input.statusText = new string [ 4 ];

            ServerConsole.Instance.input.statusText [ 3 ] = $" Carbon v{Version}, {CarbonLoader._loadedMods.Count:n0} mods, {CarbonLoader._loadedMods.Sum ( x => x.Plugins.Count ):n0} plgs";
#endif
        }

        public void Init ()
        {
            if ( IsInitialized ) return;
            IsInitialized = true;

            Format ( $"Loading..." );

            GetRootFolder ();
            GetConfigsFolder ();
            GetDataFolder ();
            GetPluginsFolder ();
            GetLogsFolder ();
            GetLangFolder ();

            OsEx.Folder.DeleteContents ( GetTempFolder () );

            _installProcessors ();

            Interface.Initialize ();

            _clearCommands ();
            _installDefaultCommands ();

            ReloadPlugins ();

            Format ( $"Loaded." );

            RefreshConsoleInfo ();
        }
        public void UnInit ()
        {
            _uninstallProcessors ();
            _clearCommands ( all: true );

            ClearPlugins ();
            CarbonLoader._loadedMods.Clear ();
            Debug.Log ( $"Unloaded Carbon." );

#if WIN
            if ( ServerConsole.Instance != null && ServerConsole.Instance.input != null )
            {
                ServerConsole.Instance.input.statusText [ 3 ] = "";
                ServerConsole.Instance.input.statusText = new string [ 3 ];
            }
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
}