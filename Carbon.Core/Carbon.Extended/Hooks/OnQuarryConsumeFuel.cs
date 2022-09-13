using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MiningQuarry ), "FuelCheck" )]
    public class OnQuarryConsumeFuel
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnQuarryConsumeFuel" );
        }
    }
}