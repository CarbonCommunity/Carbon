using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "SendUpdatedInventory" )]
    public class OnInventoryNetworkUpdate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnInventoryNetworkUpdate" );
        }
    }
}