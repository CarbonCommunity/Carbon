///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Hooks
{
    [OxideHook("OnEntityTakeDamage", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Entity)]
    [OxideHook.Parameter("entity", typeof(BaseCombatEntity))]
    [OxideHook.Parameter("info", typeof(HitInfo))]
    [OxideHook.Info("Alternatively, modify the HitInfo object to change the damage.")]
    [OxideHook.Patch(typeof(BaseCombatEntity), "OnAttacked")]
    public class BaseCombatEntity_OnAttacked
    {
        public static bool Prefix(HitInfo info, ref BaseCombatEntity __instance)
        {
            var obj = HookCaller.CallStaticHook("OnEntityTakeDamage", __instance, info);

            if (obj is bool)
            {
                return (bool)obj;
            }

            return true;
        }
    }
}
