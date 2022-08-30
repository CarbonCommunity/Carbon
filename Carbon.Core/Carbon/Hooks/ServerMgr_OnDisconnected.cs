using Harmony;
using Carbon.Core.Harmony;

[HarmonyPatch ( typeof ( ServerMgr ), "OnDisconnected" )]
public class ServerMgr_OnDisconnected
{
    public static void Postfix ( string strReason, Network.Connection connection )
    {
        HookExecutor.CallStaticHook ( "OnPlayerDisconnected", connection.player as BasePlayer, strReason );
    }
}