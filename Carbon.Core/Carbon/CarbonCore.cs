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
        public static CarbonCore Instance;
        public string Id;
        public RustPlugin CorePlugin;

        internal static MethodInfo _getMod { get; } = typeof ( HarmonyLoader ).GetMethod ( "GetMod", BindingFlags.Static | BindingFlags.NonPublic );

        public List<OxideCommand> AllChatCommands { get; } = new List<OxideCommand> ();
        public List<OxideCommand> AllConsoleCommands { get; } = new List<OxideCommand> ();

        public static VersionNumber Version { get; } = new VersionNumber ( 1, 0, 0 );

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

        public static void Log ( object message )
        {
            Debug.Log ( $"[Carbon v{Version}] {message}" );
        }
        public static void Warn ( object message )
        {
            Debug.LogWarning ( $"[Carbon v{Version}] {message}" );
        }
        public static void Error ( object message, Exception exception = null )
        {
            if ( exception == null ) Debug.LogError ( message );
            else Debug.LogException ( new Exception ( $"[Carbon v{Version}] {message}", exception ) );
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
        }
        public static void ClearPlugins ()
        {
            Instance._clearCommands ();
            CarbonLoader.UnloadCarbonMods ();
        }

        internal void _clearCommands ()
        {
            AllChatCommands.RemoveAll ( x => !x.Plugin.IsCorePlugin );
            AllConsoleCommands.RemoveAll ( x => !x.Plugin.IsCorePlugin );
        }
        internal void _installDefaultCommands ()
        {
            CorePlugin = new CarbonCorePlugin { Name = "Core", IsCorePlugin = true };

            CarbonLoader.ProcessCommands ( typeof ( CarbonCorePlugin ), CorePlugin, prefix: "c" );
        }

        public void Init ()
        {
            Format ( $"Loading..." );

            GetRootFolder ();
            GetConfigsFolder ();
            GetDataFolder ();
            GetPluginsFolder ();
            GetLogsFolder ();
            GetLangFolder ();

            Interface.Initialize ();

            _clearCommands ();
            _installDefaultCommands ();

            ReloadPlugins ();

            Format ( $"Loaded." );
        }
    }

    public class Initalizer : IHarmonyModHooks
    {
        public void OnLoaded ( OnHarmonyModLoadedArgs args )
        {
            CarbonCore.Format ( "Initializing..." );

            var newId = Assembly.GetExecutingAssembly ().GetName ().Name;

            if ( CarbonCore.Instance != null )
            {
                CarbonCore.WarnFormat ( $"Old: {CarbonCore.Instance.Id} New: {newId}" );

                if ( CarbonCore.Instance.Id != newId )
                {
                    HarmonyLoader.TryUnloadMod ( CarbonCore.Instance.Id );
                    CarbonCore.WarnFormat ( $"Unloaded previous: {CarbonCore.Instance.Id}" );
                    CarbonCore.Instance = null;
                }
            }

            if ( CarbonCore.Instance == null )
            {
                CarbonCore.Instance = new CarbonCore ();
                CarbonCore.Instance.Init ();

                CarbonCore.Instance.Id = newId;
            }
        }

        public void OnUnloaded ( OnHarmonyModUnloadedArgs args )
        {
            CarbonCore.ClearPlugins ();
            Debug.Log ( $"Unloaded Carbon." );
        }
    }
}