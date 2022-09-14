using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingItem" )]
    public class OnLootItem
    {
        public static void Postfix ( Item item , ref PlayerLoot __instance )
        {
            HookExecutor.CallStaticHook ( "OnLootItem" );
        }
    }
}