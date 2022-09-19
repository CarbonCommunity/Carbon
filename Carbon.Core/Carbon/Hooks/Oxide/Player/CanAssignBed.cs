using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "CanAssignBed", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "this", typeof ( SleepingBag ) )]
    [Hook.Parameter ( "friendId", typeof ( ulong ) )]
    [Hook.Info ( "Called when a player attempts to assign a bed or sleeping bag to another player." )]
    [Hook.Patch ( typeof ( SleepingBag ), "AssignToFriend" )]
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