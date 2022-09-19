using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnEntityDeath" ), Hook.Category ( Hook.Category.Enum.Entity )]
    [Hook.Parameter ( "this", typeof ( BaseCombatEntity ) )]
    [Hook.Parameter ( "info", typeof ( HitInfo ) )]
    [Hook.Info ( "HitInfo might be null, check it before use." )]
    [Hook.Info ( "Editing hitInfo has no effect because the death has already happened." )]
    [Hook.Patch ( typeof ( BaseCombatEntity ), "Die" )]
    public class BaseCombatEntity_Die
    {
        public static void Prefix ( HitInfo info, ref BaseCombatEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath", __instance, info );
        }
    }

    [Hook ( "OnEntityDeath" ), Hook.Category ( Hook.Category.Enum.Entity )]
    [Hook.Parameter ( "this", typeof ( ResourceEntity ) )]
    [Hook.Parameter ( "info", typeof ( HitInfo ) )]
    [Hook.Info ( "HitInfo might be null, check it before use." )]
    [Hook.Info ( "Editing hitInfo has no effect because the death has already happened." )]
    [Hook.Patch ( typeof ( ResourceEntity ), "OnKilled" )]
    public class ResourceEntity_OnKilled
    {
        public static void Prefix ( HitInfo info, ref ResourceEntity __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath", __instance, info );
        }
    }
}