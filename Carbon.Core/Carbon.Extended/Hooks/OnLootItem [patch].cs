using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingItem" )]
    public class OnLootItem [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootItem [patch]" );
        }
    }
}