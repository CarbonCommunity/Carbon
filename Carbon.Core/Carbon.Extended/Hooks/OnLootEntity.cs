using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingEntity" )]
    public class OnLootEntity
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootEntity" );
        }
    }
}