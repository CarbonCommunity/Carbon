///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamInvite", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Team )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Useful for canceling sending an invitation." )]
    [OxideHook.Patch ( typeof ( RelationshipManager ), "sendinvite" )]
    public class RelationshipManager_sendinvite
    {
        public static bool Prefix ( ConsoleSystem.Arg arg )
        {
            var basePlayer = arg.Player ();
            var playerTeam = RelationshipManager.ServerInstance.FindTeam ( basePlayer.currentTeam );

            if ( playerTeam == null )
            {
                return true;
            }

            if ( playerTeam.GetLeader () == null )
            {
                return true;
            }

            if ( playerTeam.GetLeader () != basePlayer )
            {
                return true;
            }

            RaycastHit hit;
            if ( UnityEngine.Physics.Raycast ( basePlayer.eyes.position, basePlayer.eyes.HeadForward (), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore ) )
            {
                var entity = hit.GetEntity ();

                if ( entity != null )
                {
                    var component = entity.GetComponent<BasePlayer> ();

                    if ( component && component != basePlayer && !component.IsNpc && component.currentTeam == 0UL )
                    {
                        if ( Interface.CallHook ( "OnTeamInvite", basePlayer, component ) != null )
                        {
                            return false;
                        }

                        playerTeam.SendInvite ( component );
                    }
                }
            }

            return false;
        }
    }
}