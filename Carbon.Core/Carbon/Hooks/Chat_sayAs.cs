using Carbon.Core;
using ConVar;
using Harmony;
using System.Linq;

[HarmonyPatch ( typeof ( Chat ), "sayAs" )]
public class Chat_SayAs
{
    public static bool Prefix ( Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null )
    {
        var split = message.Substring ( 1 ).Split ( ' ' );
        var command = split [ 0 ];
        var args = split.Skip ( 1 ).ToArray ();
        
            foreach (var cmd in CarbonCore.Instance?.AllChatCommands)
            {
                if (cmd.Command == command)
                {
                    cmd.Callback?.Invoke(player, command, args);
                    return false;
                }
            }

        return true;
    }
}