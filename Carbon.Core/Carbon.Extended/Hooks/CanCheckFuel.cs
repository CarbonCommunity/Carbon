using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EntityFuelSystem ), "IsInFuelInteractionRange" )]
    public class CanCheckFuel
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanCheckFuel" );
        }
    }
}