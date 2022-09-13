using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseRidableAnimal ), "RPC_OpenLoot" )]
    public class CanLootEntity [BaseRidableAnimal]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity [BaseRidableAnimal]" );
        }
    }
}