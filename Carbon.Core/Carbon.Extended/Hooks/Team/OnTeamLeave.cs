using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamLeave", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Useful for canceling the leave from the team." )]
    [HarmonyPatch ( typeof ( RelationshipManager ), "leaveteam" )]
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