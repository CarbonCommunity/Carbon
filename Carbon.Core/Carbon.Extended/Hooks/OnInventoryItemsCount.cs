using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "GetAmount" )]
    public class OnInventoryItemsCount
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnInventoryItemsCount" );
        }
    }
}