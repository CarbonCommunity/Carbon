using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "OnDisconnected" )]
    public class OnPlayerDisconnected
    {
        public static void Postfix ( System.String strReason, Network.Connection connection )
        {
            HookExecutor.CallStaticHook ( "OnPlayerDisconnected" );
        }
    }
}