using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "RPC_KeepAlive" )]
    public class OnPlayerKeepAlive
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerKeepAlive" );
        }
    }
}