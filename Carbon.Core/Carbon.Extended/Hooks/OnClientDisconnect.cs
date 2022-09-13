using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "ReadDisconnectReason" )]
    public class OnClientDisconnect
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnClientDisconnect" );
        }
    }
}