using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EntityFuelSystem ), "TryUseFuel" )]
    public class CanUseFuel
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanUseFuel" );
        }
    }
}