using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnHammerHit" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "hitInfo", typeof ( HitInfo ) )]
    [Hook.Info ( "Called when the player has hit something with a hammer." )]
    [HarmonyPatch ( typeof ( Hammer ), "DoAttackShared" )]
    public class Hammer_DoAttackShared
    {
        public static bool Prefix ( HitInfo info, ref Hammer __instance )
        {
            return HookExecutor.CallStaticHook ( "OnHammerHit", __instance.GetOwnerPlayer (), info ) == null;
        }
    }
}