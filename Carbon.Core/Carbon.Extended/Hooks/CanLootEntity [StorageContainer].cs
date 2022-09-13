using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( StorageContainer ), "PlayerOpenLoot" )]
    public class CanLootEntity [StorageContainer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity [StorageContainer]" );
        }
    }
}