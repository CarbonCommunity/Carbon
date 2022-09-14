using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( StorageContainer ), "PlayerStoppedLooting" )]
    public class OnLootEntityEnd
    {
        public static bool Prefix ( BasePlayer player , ref StorageContainer __instance )
        {
            return HookExecutor.CallStaticHook ( "OnLootEntityEnd", a0, __instance ) == null;
        }
    }
}