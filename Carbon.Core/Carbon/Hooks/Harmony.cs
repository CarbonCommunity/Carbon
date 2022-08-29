using ConVar;
using Facepunch;
using Harmony;
using Carbon.Core.Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Carbon.Core;

[HarmonyPatch ( typeof ( ConVar.Harmony ), "Load" )]
public class Harmony_Load
{
    public const string CARBON_LOADED = nameof ( CARBON_LOADED );

    public static bool Prefix ( ConsoleSystem.Arg args )
    {
        if ( !args.FullString.StartsWith ( "Carbon" ) ) return true;

        var oldMod = PlayerPrefs.GetString ( CARBON_LOADED );
        var mod = args.Args != null && args.Args.Length > 0 ? args.Args [ 0 ] : null;

        if ( oldMod == mod )
        {
            CarbonCore.Warn ( $"An instance of Carbon v{CarbonCore.Version} is already loaded." );
            return false;
        }
        else
        {
            CarbonCore.Instance?.UnInit ();
            HarmonyLoader.TryUnloadMod ( oldMod );
            CarbonCore.WarnFormat ( $"Unloaded previous: {oldMod}" );
            CarbonCore.Instance = null;
        }

        PlayerPrefs.SetString ( CARBON_LOADED, mod );

        return true;
    }
}

[HarmonyPatch ( typeof ( ConVar.Harmony ), "Unload" )]
public class Harmony_Unload
{
    public static void Prefix ( ConsoleSystem.Arg args )
    {
        if ( !args.FullString.StartsWith ( "Carbon" ) ) return;

        CarbonCore.Log ( "Intentional unload happened." );

        PlayerPrefs.SetString ( Harmony_Load.CARBON_LOADED, string.Empty );
        CarbonCore.Instance?.UnInit ();
        CarbonCore.Instance = null;
    }
}