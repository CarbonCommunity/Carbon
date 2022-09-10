using Carbon.Core;
using Carbon.Core.Extensions;
using ConVar;
using Harmony;
using System;
using System.Linq;

[HarmonyPatch ( typeof ( Chat ), "sayAs" )]
public class Chat_SayAs
{
    public static bool Prefix ( Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null )
    {
        if ( CarbonCore.Instance == null ) return true;

        try
        {
            var split = message.Substring ( 1 ).Split ( ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries );
            var command = split [ 0 ].Trim ();
            var args = split.Length > 1 ? split.Skip ( 1 ).ToArray () : null;

            foreach ( var cmd in CarbonCore.Instance?.AllChatCommands )
            {
                if ( cmd.Command == command )
                {
                    cmd.Callback?.Invoke ( player, command, args );
                    return false;
                }
            }
        }
        catch { }

        return true;
    }
}