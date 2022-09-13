using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EntityFuelSystem ), "HasFuel" )]
    public class OnFuelCheck
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFuelCheck" );
        }
    }
}