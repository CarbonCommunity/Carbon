using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "CreateWorldProjectile" )]
    public class CanCreateWorldProjectile
    {
        public static bool Prefix ( HitInfo info, ItemDefinition itemDef, ItemModProjectile itemMod, Projectile projectilePrefab, Item recycleItem )
        {
            return HookExecutor.CallStaticHook ( "CanCreateWorldProjectile", a0, a1 ) == null;
        }
    }
}