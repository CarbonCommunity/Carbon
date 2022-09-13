using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EntityFuelSystem ), "GetFuelAmount" )]
    public class OnFuelAmountCheck
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFuelAmountCheck" );
        }
    }
}