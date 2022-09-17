using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "CanTakeCutting", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Resources )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( GrowableEntity ) )]
    [Hook.Info ( "Called when a player is trying to take a cutting (clone) of a GrowableEntity." )]
    [HarmonyPatch ( typeof ( GrowableEntity ), "TakeClones" )]
    public class GrowableEntity_TakeClones
    {
        public static bool Prefix ( BasePlayer player, ref GrowableEntity __instance )
        {
            if ( player == null )
            {
                return false;
            }
            if ( !__instance.CanClone () )
            {
                return false;
            }

            return HookExecutor.CallStaticHook ( "CanTakeCutting", player, __instance ) == null;
        }
    }
}