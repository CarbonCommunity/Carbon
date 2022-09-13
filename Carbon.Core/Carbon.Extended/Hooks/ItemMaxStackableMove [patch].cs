using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "MoveItem" )]
    public class ItemMaxStackableMove [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "ItemMaxStackableMove [patch]" );
        }
    }
}