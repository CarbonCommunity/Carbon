using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "ClientReady" )]
    public class OnPlayerSetInfo [server]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSetInfo [server]" );
        }
    }
}