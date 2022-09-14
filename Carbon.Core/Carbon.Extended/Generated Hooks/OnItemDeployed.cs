using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Deployer ), "DoDeploy_Slot" )]
    public class OnItemDeployed
    {
        public static void Postfix ( Deployable deployable, UnityEngine.Ray ray, System.UInt32 entityID , ref Deployer __instance )
        {
            HookExecutor.CallStaticHook ( "OnItemDeployed" );
        }
    }
}