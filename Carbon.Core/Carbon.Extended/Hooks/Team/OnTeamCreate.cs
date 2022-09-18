using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamCreate", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Useful for canceling team creation." )]
    [HarmonyPatch ( typeof ( RelationshipManager ), "trycreateteam" )]
    public class RelationshipManager_trycreateteam_OnTeamCreate
    {
        public static bool Prefix ( ConsoleSystem.Arg arg )
        {
            if ( RelationshipManager.maxTeamSize == 0 )
            {
                arg.ReplyWith ( "Teams are disabled on this server" );
                return false;
            }

            var basePlayer = arg.Player ();
            if ( basePlayer.currentTeam != 0UL )
            {
                return false;
            }

            if ( HookExecutor.CallStaticHook ( "OnTeamCreate", basePlayer ) != null )
            {
                return false;
            }

            var playerTeam = RelationshipManager.ServerInstance.CreateTeam ();
            var playerTeam2 = playerTeam;
            playerTeam2.teamLeader = basePlayer.userID;
            playerTeam2.AddPlayer ( basePlayer );
            HookExecutor.CallStaticHook ( "OnTeamCreated", basePlayer, playerTeam );

            return false;
        }
    }
}