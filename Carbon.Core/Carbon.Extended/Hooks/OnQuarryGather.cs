using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MiningQuarry ), "ProcessResources" )]
    public class OnQuarryGather
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnQuarryGather" );
        }
    }
}