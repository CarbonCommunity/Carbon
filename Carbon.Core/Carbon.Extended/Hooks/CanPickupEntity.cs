using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "CanPickup" )]
    public class CanPickupEntity
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanPickupEntity" );
        }
    }
}