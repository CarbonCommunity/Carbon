using Carbon.Core;
using Carbon.Core.Extensions;
using Facepunch;
using Harmony;
using System;
using System.Linq;

[HarmonyPatch ( typeof ( RCon ), "OnCommand" )]
public class RconCommand
{
    public static bool Prefix ( RCon.Command cmd )
    {
        if ( CarbonCore.Instance == null ) return true;

        try
        {
            var split = cmd.Message.Split ( ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries );
            var command = split [ 0 ].Trim ();
            var args2 = split.Length > 1 ? split.Skip ( 1 ).ToArray () : null;

            foreach ( var carbonCommand in CarbonCore.Instance.AllConsoleCommands )
            {
                if ( carbonCommand.Command == command )
                {
                    try
                    {
                        carbonCommand.Callback?.Invoke ( null, command, args2 );
                        return !carbonCommand.SkipOriginal;
                    }
                    catch ( Exception ex )
                    {
                        CarbonCore.Error ( "RconCommand_OnCommand", ex );
                    }

                    break;
                }
            }
        }
        catch { }

        return true;
    }
}