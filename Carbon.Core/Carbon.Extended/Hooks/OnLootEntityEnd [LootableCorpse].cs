using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LootableCorpse ), "PlayerStoppedLooting" )]
    public class OnLootEntityEnd [LootableCorpse]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd [LootableCorpse]" );
        }
    }
}