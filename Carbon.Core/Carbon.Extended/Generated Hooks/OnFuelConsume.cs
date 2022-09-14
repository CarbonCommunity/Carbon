using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseOven ), "ConsumeFuel" )]
    public class OnFuelConsume
    {
        public static bool Prefix ( Item fuel, ItemModBurnable burnable )
        {
            return HookExecutor.CallStaticHook ( "OnFuelConsume" ) == null;
        }
    }
}