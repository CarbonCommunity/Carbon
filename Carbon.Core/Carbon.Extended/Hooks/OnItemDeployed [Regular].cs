using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Deployer ), "DoDeploy_Regular" )]
    public class OnItemDeployed [Regular]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemDeployed [Regular]" );
        }
    }
}