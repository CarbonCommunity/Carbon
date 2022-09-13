using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "FindAmmo" )]
    public class OnInventoryAmmoFind
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnInventoryAmmoFind" );
        }
    }
}