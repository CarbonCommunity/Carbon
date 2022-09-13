using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CargoShip ), "StartEgress" )]
    public class OnCargoShipEgress
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCargoShipEgress" );
        }
    }
}