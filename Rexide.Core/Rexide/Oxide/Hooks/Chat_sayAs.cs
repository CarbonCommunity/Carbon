using ConVar;
using Harmony;
using System.Linq;
using UnityEngine;

[HarmonyPatch ( typeof ( Chat ), "sayAs" )]
public class Chat_SayAs
{
    public static void Prefix ( Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null )
    {
        Debug.Log ( $"TEST: {username}: {message}" );

        var split = message.Substring ( 1 ).Split ( ' ' );
        var command = split [ 0 ];
        var args = split.Skip ( 1 ).ToArray ();

        foreach ( var cmd in Rexide.Instance?.AllChatCommands )
        {
            if ( cmd.Command == command )
            {
                cmd.Callback?.Invoke ( player, command, args );
            }
        }
    }
}