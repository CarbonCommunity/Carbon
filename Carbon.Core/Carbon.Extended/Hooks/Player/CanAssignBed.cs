using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "AssignToFriend" )]
    public class SleepingBag_CanAffordUpgrade
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref SleepingBag __instance )
        {
            if ( !msg.player.CanInteract () )
            {
                return true;
            }

            if ( __instance.deployerUserID != msg.player.userID )
            {
                return true;
            }

            var oldPosition = msg.read.Position;
            var userId = msg.read.UInt64 ();
            msg.read.Position = oldPosition;

            if ( userId == 0UL )
            {
                return true;
            }

            return HookExecutor.CallStaticHook ( "CanAssignBed", msg.player, __instance, userId ) == null;
        }
    }
}