using ConVar;
using Facepunch;
using Harmony;
using System;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( ConsoleSystem ), "Run" )]
public class ConsoleSystem_Run
{
    public static void Prefix ( ConsoleSystem.Option options, string strCommand, object [] args, ref string __result )
    {
        Rexide.Log ( $"ConsoleSystem_Run: {strCommand} {string.Join( " ", args )}" );

        var args2 = Facepunch.Pool.GetList<string>();
        foreach(var arg in args) args.Add( arg );
        var args3 = args2.ToArray ();

        foreach ( var cmd in Rexide.Instance?.AllConsoleCommands )
        {
            if ( cmd.Command == strCommand )
            {
                try
                {
                    cmd.Callback?.Invoke ( options.Connection?.player as BasePlayer, strCommand, args3 );
                }
                catch ( Exception ex )
                {
                    Rexide.Error ( "ConsoleSystem_Run", ex );
                }
            }
        }

        Facepunch.Pool.Free ( ref args3 );
        Facepunch.Pool.FreeList ( ref args2 );
    }
}