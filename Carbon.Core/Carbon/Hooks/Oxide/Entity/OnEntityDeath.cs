using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "OnEntityDeath" ), OxideHook.Category ( OxideHook.Category.Enum.Entity )]
    [OxideHook.Parameter ( "this", typeof ( BaseCombatEntity ) )]
    [OxideHook.Parameter ( "info", typeof ( HitInfo ) )]
    [OxideHook.Info ( "HitInfo might be null, check it before use." )]
    [OxideHook.Info ( "Editing hitInfo has no effect because the death has already happened." )]
    [OxideHook.Patch ( typeof ( BaseCombatEntity ), "Die" )]
    public class BaseCombatEntity_Die
    {
        public static void Prefix ( HitInfo info, ref BaseCombatEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath", __instance, info );
        }
    }

    [OxideHook ( "OnEntityDeath" ), OxideHook.Category ( OxideHook.Category.Enum.Entity )]
    [OxideHook.Parameter ( "this", typeof ( ResourceEntity ) )]
    [OxideHook.Parameter ( "info", typeof ( HitInfo ) )]
    [OxideHook.Info ( "HitInfo might be null, check it before use." )]
    [OxideHook.Info ( "Editing hitInfo has no effect because the death has already happened." )]
    [OxideHook.Patch ( typeof ( ResourceEntity ), "OnKilled" )]
    public class ResourceEntity_OnKilled
    {
        public static void Prefix ( HitInfo info, ref ResourceEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath", __instance, info );
        }
    }
}