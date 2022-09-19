using Carbon.Core;
using Carbon.Core.Extensions;
using Facepunch;
using Facepunch.Extend;
using Harmony;
using System;
using System.Linq;

[HarmonyPatch ( typeof ( ConsoleSystem ), "Run" )]
public class ConsoleCommand
{
    public static bool Prefix ( ConsoleSystem.Option options, string strCommand, object [] args )
    {
        if ( CarbonCore.Instance == null ) return true;

        try
        {
            var split = strCommand.Split ( ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries );
            var command = split [ 0 ].Trim ();
            var args2 = split.Length > 1 ? strCommand.Substring ( command.Length + 1 ).SplitQuotesStrings () : null;
            Facepunch.Pool.Free ( ref split );

            foreach ( var cmd in CarbonCore.Instance.AllConsoleCommands )
            {
                if ( cmd.Command == command )
                {
                    try
                    {
                        cmd.Callback?.Invoke ( options.Connection?.player as BasePlayer, command, args2 );
                    }
                    catch ( Exception ex )
                    {
                        CarbonCore.Error ( "ConsoleSystem_Run", ex );
                    }

                    Facepunch.Pool.Free ( ref args2 );
                    return !cmd.SkipOriginal;
                }
            }

        }
        catch { }

        return true;
    }
}