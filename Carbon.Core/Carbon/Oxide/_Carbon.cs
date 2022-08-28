using Oxide.Core;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

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
        var folder = Path.Combine ( $"{Application.dataPath}\\..", "carbon" );
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

    public static void ReloadPlugins ()
    {
        CarbonLoader.LoadCarbonMods ();
    }
    public static void ClearPlugins ()
    {
        CarbonLoader.UnloadCarbonMods ();
    }

    internal void _clearCommands ()
    {
        AllChatCommands.Clear ();
        AllConsoleCommands.Clear ();
    }
    internal void _installDefaultCommands ()
    {
        var cmd = new OxideCommand
        {
            Command = "carbon",
            Plugin = CorePlugin = new RustPlugin { Name = "Core" },
            Callback = ( player, command, args2 ) =>
            {
                player.ChatMessage ( $"You're running <color=orange>Carbon v{CarbonCore.Version}</color>" );
            }
        };

        AllChatCommands.Add ( cmd );
        AllConsoleCommands.Add ( cmd );
    }

    public void Init ()
    {
        Log ( $"Loading..." );

        GetRootFolder ();
        GetConfigsFolder ();
        GetDataFolder ();
        GetPluginsFolder ();
        GetLogsFolder ();

        _clearCommands ();
        _installDefaultCommands ();

        Log ( $"Loaded." );

        ReloadPlugins ();
    }
}

public class Initalizer : IHarmonyModHooks
{
    public void OnLoaded ( OnHarmonyModLoadedArgs args )
    {
        CarbonCore.Log ( "Initializing..." );

        var newId = Assembly.GetExecutingAssembly ().GetName ().Name;

        if ( CarbonCore.Instance != null )
        {
            if ( CarbonCore.Instance.Id != newId )
            {
                HarmonyLoader.TryUnloadMod ( CarbonCore.Instance.Id );
                CarbonCore.Warn ( $"Unloaded previous: {CarbonCore.Instance.Id}" );
            }
        }

        if ( CarbonCore.Instance == null )
        {
            CarbonCore.Instance = new CarbonCore ();
            CarbonCore.Instance.Init ();
        }

        CarbonCore.Instance.Id = newId;
    }

    public void OnUnloaded ( OnHarmonyModUnloadedArgs args )
    {
        CarbonCore.ClearPlugins ();
        Debug.Log ( $"Unloaded Carbon." );
    }
}