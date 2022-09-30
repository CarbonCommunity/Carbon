///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Harmony;
using UnityEngine;
using Carbon.Core;

[HarmonyPatch ( typeof ( ConVar.Harmony ), "Load" )]
public class Harmony_Load
{
    public const string CARBON_LOADED = nameof ( CARBON_LOADED );

    public static bool Prefix ( ConsoleSystem.Arg args )
    {
        var mod = args.Args != null && args.Args.Length > 0 ? args.Args [ 0 ] : null;

        if ( !mod.Equals ( "carbon", System.StringComparison.OrdinalIgnoreCase ) &&
             !mod.Equals ( "carbon-unix", System.StringComparison.OrdinalIgnoreCase ) ) return true;

        if ( string.IsNullOrEmpty ( mod ) || 
            !mod.StartsWith ( "carbon", System.StringComparison.OrdinalIgnoreCase ) 
            || CarbonCore.IsAddon ( mod ) ) return true;

        var oldMod = PlayerPrefs.GetString ( CARBON_LOADED );

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
    public static bool Prefix ( ConsoleSystem.Arg args )
    {
        var mod = args.Args != null && args.Args.Length > 0 ? args.Args [ 0 ] : null;

        if ( string.IsNullOrEmpty ( mod ) ) return true;

        if ( mod.Equals ( "carbon", System.StringComparison.OrdinalIgnoreCase ) ||
             mod.Equals ( "carbon-unix", System.StringComparison.OrdinalIgnoreCase ) ) 
             mod =
#if WIN
                "Carbon";
#elif UNIX
                "Carbon-Unix";
#endif

        if ( !mod.StartsWith ( "carbon", System.StringComparison.OrdinalIgnoreCase )
            || CarbonCore.IsAddon ( mod ) ) return true;

        PlayerPrefs.SetString ( Harmony_Load.CARBON_LOADED, string.Empty );
        CarbonCore.Instance?.UnInit ();
        CarbonCore.Instance = null;

        HarmonyLoader.TryUnloadMod ( mod );
        return false;
    }
}