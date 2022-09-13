using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "FindItemIDs" )]
    public class OnInventoryItemsFind
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnInventoryItemsFind" );
        }
    }
}