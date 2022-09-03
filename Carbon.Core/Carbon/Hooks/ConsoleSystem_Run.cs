using Carbon.Core;
using Harmony;
using System;
using System.Linq;

[HarmonyPatch ( typeof ( ConsoleSystem ), "Run" )]
public class ConsoleSystem_Run
{
    public static bool Prefix ( ConsoleSystem.Option options, string strCommand, object [] args )
    {
        try
        {
            var split = strCommand.Split ( ' ' );
            var command = split [ 0 ];
            var args2 = split.Skip ( 1 ).ToArray ();

            if ( CarbonCore.Instance == null ) return true;

            foreach ( var cmd in CarbonCore.Instance.AllConsoleCommands )
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
        }
        catch { }

        return true;
    }
}