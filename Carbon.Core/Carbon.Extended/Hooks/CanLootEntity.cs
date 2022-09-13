using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseRidableAnimal ), "RPC_OpenLoot" )]
    public class CanLootEntity
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity" );
        }
    }
}