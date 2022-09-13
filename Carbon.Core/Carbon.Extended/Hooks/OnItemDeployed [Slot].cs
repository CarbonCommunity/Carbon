using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Deployer ), "DoDeploy_Slot" )]
    public class OnItemDeployed [Slot]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemDeployed [Slot]" );
        }
    }
}