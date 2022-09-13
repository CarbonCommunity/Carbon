using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LootContainer ), "SpawnLoot" )]
    public class OnLootSpawn [LootContainer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootSpawn [LootContainer]" );
        }
    }
}