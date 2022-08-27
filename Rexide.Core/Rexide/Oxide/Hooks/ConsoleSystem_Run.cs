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
    internal static List<string> _argumentBuffer = new List<string> ( 10 );

    public static void Prefix ( ConsoleSystem.Option options, string strCommand, object [] args, ref string __result )
    {

        var split = strCommand.Split ( ' ' );
        var command = split [ 0 ];
        var args2 = split.Skip ( 1 ).ToArray ();

        Rexide.Log ( $"'{strCommand}' | '{string.Join ( " ", args2 )}'" );


        foreach ( var cmd in Rexide.Instance?.AllConsoleCommands )
        {
            if ( cmd.Command == command )
            {
                try
                {
                    cmd.Callback?.Invoke ( options.Connection?.player as BasePlayer, command, args2 );
                }
                catch ( Exception ex )
                {
                    Rexide.Error ( "ConsoleSystem_Run", ex );
                }

                break;
            }
        }
    }
}