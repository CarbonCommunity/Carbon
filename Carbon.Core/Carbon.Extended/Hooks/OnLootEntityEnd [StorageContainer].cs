using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( StorageContainer ), "PlayerStoppedLooting" )]
    public class OnLootEntityEnd [StorageContainer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd [StorageContainer]" );
        }
    }
}