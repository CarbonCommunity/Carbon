using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DroppedItemContainer ), "RPC_OpenLoot" )]
    public class CanLootEntity [DroppedItemContainer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity [DroppedItemContainer]" );
        }
    }
}