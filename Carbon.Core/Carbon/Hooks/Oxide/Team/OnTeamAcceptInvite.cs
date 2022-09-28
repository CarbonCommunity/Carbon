///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnTeamAcceptInvite", typeof ( object ) ), OxideHook.Category ( Hook.Category.Enum.Team )]
    [OxideHook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Useful for canceling team invitation acceptation." )]
    [OxideHook.Patch ( typeof ( RelationshipManager ), "acceptinvite" )]
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