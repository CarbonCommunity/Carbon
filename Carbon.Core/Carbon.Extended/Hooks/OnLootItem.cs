using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingItem" )]
    public class OnLootItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootItem" );
        }
    }
}