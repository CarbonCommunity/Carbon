using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamRejectInvite", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "rejector", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Info ( "Useful for canceling the invitation rejection." )]
    [Hook.Patch ( typeof ( RelationshipManager ), "rejectinvite" )]
    public class RelationshipManager_rejectinvite
    {
        public static bool Prefix ( ConsoleSystem.Arg arg )
        {
            var basePlayer = arg.Player ();

            if ( basePlayer == null )
            {
                return false;
            }

            if ( basePlayer.currentTeam != 0UL )
            {
                return false;
            }

            var @ulong = arg.GetULong ( 0, 0UL );
            var playerTeam = RelationshipManager.ServerInstance.FindTeam ( @ulong );

            if ( playerTeam == null )
            {
                basePlayer.ClearPendingInvite ();
                return false;
            }

            if ( Interface.CallHook ( "OnTeamRejectInvite", basePlayer, playerTeam ) != null )
            {
                return false;
            }

            playerTeam.RejectInvite ( basePlayer );
            return false;
        }
    }
}