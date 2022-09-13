using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseOven ), "ConsumeFuel" )]
    public class OnFuelConsume
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnFuelConsume" );
        }
    }
}