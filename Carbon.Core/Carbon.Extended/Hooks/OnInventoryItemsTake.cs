using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "Take" )]
    public class OnInventoryItemsTake
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnInventoryItemsTake" );
        }
    }
}