using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EntityFuelSystem ), "GetFuelItem" )]
    public class OnFuelItemCheck
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFuelItemCheck" );
        }
    }
}