using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NetworkVisibilityGrid ), "GetVisibleFrom" )]
    public class OnNetworkSubscriptionsGather
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNetworkSubscriptionsGather" );
        }
    }
}