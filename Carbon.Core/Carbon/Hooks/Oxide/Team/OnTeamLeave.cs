///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnTeamLeave", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Team )]
    [OxideHook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Useful for canceling the leave from the team." )]
    [OxideHook.Patch ( typeof ( RelationshipManager ), "leaveteam" )]
    public class RelationshipManager_leaveteam
    {
        public static bool Prefix ( ConsoleSystem.Arg arg )
        {
            var basePlayer = arg.Player ();

            if ( basePlayer == null )
            {
                return false;
            }

            if ( basePlayer.currentTeam == 0UL )
            {
                return false;
            }

            var playerTeam = RelationshipManager.ServerInstance.FindTeam ( basePlayer.currentTeam );

            if ( playerTeam != null )
            {
                if ( Interface.CallHook ( "OnTeamLeave", playerTeam, basePlayer ) != null )
                {
                    return false;
                }

                playerTeam.RemovePlayer ( basePlayer.userID );
                basePlayer.ClearTeam ();
            }

            return false;
        }
    }
}