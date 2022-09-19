using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamAcceptInvite", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Useful for canceling team invitation acceptation." )]
    [Hook.Patch ( typeof ( RelationshipManager ), "acceptinvite" )]
    public class RelationshipManager_acceptinvite
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

            if ( Interface.CallHook ( "OnTeamAcceptInvite", playerTeam, basePlayer ) != null )
            {
                return false;
            }

            playerTeam.AcceptInvite ( basePlayer );

            return false;
        }
    }
}