using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LootableCorpse ), "RPC_LootCorpse" )]
    public class CanLootEntity [LootableCorpse]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity [LootableCorpse]" );
        }
    }
}