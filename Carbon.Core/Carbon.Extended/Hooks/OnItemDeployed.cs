using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Deployer ), "DoDeploy_Slot" )]
    public class OnItemDeployed
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemDeployed" );
        }
    }
}