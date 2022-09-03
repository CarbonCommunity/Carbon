using Carbon.Core.Processors;
using Humanlights.Extensions;
using Oxide.Core;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Windows;

namespace Carbon.Core
{
    public class CarbonCore
    {
        public FileSystemWatcher PluginFolderWatcher;

        public static CarbonCore Instance { get; set; }
        public RustPlugin CorePlugin { get; set; }

        internal static MethodInfo _getMod { get; } = typeof ( HarmonyLoader ).GetMethod ( "GetMod", BindingFlags.Static | BindingFlags.NonPublic );
        internal static ConsoleInput _serverConsoleInput = ServerConsole.Instance?.GetType ()?.GetField ( "input", BindingFlags.NonPublic | BindingFlags.Instance )?.GetValue ( ServerConsole.Instance ) as ConsoleInput;
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

        public PluginProcessor PluginProcessor { get; set; } = new PluginProcessor ();
        public HarmonyProcessor HarmonyProcessor { get; set; } = new HarmonyProcessor ();

        public static VersionNumber Version { get; } = new VersionNumber ( 1, 0, 1 );

        public static string GetRootFolder ()
        {
            var folder = Path.GetFullPath ( Path.Combine ( $"{Application.dataPath}\\..", "carbon" ) );
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
            else Debug.LogException ( new Exception ( $"{message}", exception ) );
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
            PluginLoader.LoadAll ();
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
            CarbonLoader._loadedMods.Add ( new CarbonLoader.CarbonMod { Name = "Carbon Community", IsCoreMod = true, Plugins = new List<RustPlugin> { CorePlugin } } );

            CarbonLoader.ProcessCommands ( typeof ( CarbonCorePlugin ), CorePlugin, prefix: "c" );
        }
        internal void _installProcessors ()
        {
            _uninstallProcessors ();

            var gameObject = new GameObject ( "PluginProcessor" );
            PluginProcessor = gameObject.AddComponent<PluginProcessor> ();
            HarmonyProcessor = gameObject.AddComponent<HarmonyProcessor> ();
        }
        internal void _registerProcessors ()
        {
            PluginProcessor?.Start ();
            HarmonyProcessor?.Start ();
        }
        internal void _uninstallProcessors ()
        {
            try
            {
                PluginProcessor?.Dispose ();
                HarmonyProcessor?.Dispose ();
            }
            catch { }

            try
            {
                var obj = PluginProcessor == null ? null : PluginProcessor.gameObject;
                if ( obj != null ) UnityEngine.Object.Destroy ( obj );
            }
            catch { }

            try
            {
                if ( PluginProcessor != null ) UnityEngine.Object.DestroyImmediate ( PluginProcessor );
                if ( HarmonyProcessor != null ) UnityEngine.Object.DestroyImmediate ( HarmonyProcessor );
            }
            catch { }
        }

        public void RefreshConsoleInfo ()
        {
            if ( _serverConsoleInput != null ) _serverConsoleInput.statusText [ 3 ] = $" Carbon v{Version}, {CarbonLoader._loadedMods.Count:n0} mods, {CarbonLoader._loadedMods.Sum ( x => x.Plugins.Count ):n0} plgs";
        }

        public void Init ()
        {       
            Format ( $"Loading..." );

            if ( _serverConsoleInput != null )
            {
                _serverConsoleInput.statusText = new string [ 4 ];
                _serverConsoleInput.statusText [ 3 ] = " Carbon Initializing...";
            }

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
            if ( _serverConsoleInput != null )
            {
                _serverConsoleInput.statusText = new string [ 3 ];
            }

            _uninstallProcessors ();
            _clearCommands ( all: true );

            ClearPlugins ();
            CarbonLoader._loadedMods.Clear ();
            Debug.Log ( $"Unloaded Carbon." );
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