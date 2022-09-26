///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnTeamKick", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Team )]
    [OxideHook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "target", typeof ( ulong ) )]
    [OxideHook.Info ( "Useful for canceling kick of the player from the team." )]
    [OxideHook.Patch ( typeof ( RelationshipManager ), "kickmember" )]
    public class RelationshipManager_kickmember
    {
        public static bool Prefix ( ConsoleSystem.Arg arg )
        {
            var basePlayer = arg.Player ();

            if ( basePlayer == null )
            {
                return false;
            }

            var playerTeam = RelationshipManager.ServerInstance.FindTeam ( basePlayer.currentTeam );

            if ( playerTeam == null )
            {
                return false;
            }

            if ( playerTeam.GetLeader () != basePlayer )
            {
                return false;
            }

            var @ulong = arg.GetULong ( 0, 0UL );

            if ( basePlayer.userID == @ulong )
            {
                return false;
            }

            if ( Interface.CallHook ( "OnTeamKick", playerTeam, basePlayer, @ulong ) != null )
            {
                return false;
            }

            playerTeam.RemovePlayer ( @ulong );
            return false;
        }
    }
}