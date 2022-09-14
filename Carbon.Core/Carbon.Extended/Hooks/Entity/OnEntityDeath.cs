using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "Die" )]
    public class BaseCombatEntity_Die
    {
        public static void Prefix ( HitInfo info, ref BaseCombatEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath", __instance, info );
        }
    }

    [HarmonyPatch ( typeof ( ResourceEntity ), "OnKilled" )]
    public class ResourceEntity_OnKilled
    {
        public static void Prefix ( HitInfo info, ref ResourceEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath", __instance, info );
        }
    }
}
