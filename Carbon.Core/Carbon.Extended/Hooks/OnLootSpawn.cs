using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LootContainer ), "SpawnLoot" )]
    public class OnLootSpawn
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLootSpawn" );
        }
    }
}