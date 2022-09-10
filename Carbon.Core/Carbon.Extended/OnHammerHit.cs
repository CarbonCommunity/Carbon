using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Hammer ), "DoAttackShared" )]
    public class Hammer_DoAttackShared
    {
        public static bool Prefix ( HitInfo info, ref Hammer __instance )
        {
            return HookExecutor.CallStaticHook ( "OnHammerHit", __instance.GetOwnerPlayer (), info ) == null;
        }
    }
}