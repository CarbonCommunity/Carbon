using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamPromote", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Parameter ( "newLeader", typeof ( BasePlayer ) )]
    [Hook.Info ( "Useful for canceling player's promotion in the team." )]
    [Hook.Patch ( typeof ( RelationshipManager ), "promote" )]
    public class RelationshipManager_promote
    {
        public static bool Prefix ( ConsoleSystem.Arg arg )
        {
            var basePlayer = arg.Player ();

            if ( basePlayer.currentTeam == 0UL )
            {
                return false;
            }

            var lookingAtPlayer = RelationshipManager.GetLookingAtPlayer ( basePlayer );

            if ( lookingAtPlayer == null )
            {
                return false;
            }

            if ( lookingAtPlayer.IsDead () )
            {
                return false;
            }

            if ( lookingAtPlayer == basePlayer )
            {
                return false;
            }

            if ( lookingAtPlayer.currentTeam == basePlayer.currentTeam )
            {
                var playerTeam = RelationshipManager.ServerInstance.teams [ basePlayer.currentTeam ];

                if ( playerTeam != null && playerTeam.teamLeader == basePlayer.userID )
                {
                    if ( Interface.CallHook ( "OnTeamPromote", playerTeam, lookingAtPlayer ) != null )
                    {
                        return false;
                    }

                    playerTeam.SetTeamLeader ( lookingAtPlayer.userID );
                }
            }

            return false;
        }
    }
}