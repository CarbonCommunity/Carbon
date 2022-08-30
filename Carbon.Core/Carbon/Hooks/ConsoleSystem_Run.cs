using Carbon.Core;
using ConVar;
using Facepunch;
using Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( ConsoleSystem ), "Run" )]
public class ConsoleSystem_Run
{
    public static bool Prefix ( ConsoleSystem.Option options, string strCommand, object [] args, ref string __result )
    {
        var split = strCommand.Split ( ' ' );
        var command = split [ 0 ];
        var args2 = split.Skip ( 1 ).ToArray ();

        foreach ( var cmd in CarbonCore.Instance?.AllConsoleCommands )
        {
            if ( cmd.Command == command )
            {
                try
                {
                    cmd.Callback?.Invoke ( options.Connection?.player as BasePlayer, command, args2 );
                    return !cmd.SkipOriginal;
                }
                catch ( Exception ex )
                {
                    CarbonCore.Error ( "ConsoleSystem_Run", ex );
                }

                break;
            }
        }

        return true;
    }
}