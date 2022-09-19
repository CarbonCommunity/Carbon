using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamDisband", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Info ( "Useful for canceling team disbandment." )]
    [Hook.Patch ( typeof ( RelationshipManager ), "DisbandTeam" )]
    public class RelationshipManager_DisbandTeam_OnTeamDisband
    {
        public static bool Prefix ( ref RelationshipManager.PlayerTeam teamToDisband, ref RelationshipManager __instance )
        {
            if ( Interface.CallHook ( "OnTeamDisband", teamToDisband ) != null )
            {
                return false;
            }

            __instance.teams.Remove ( teamToDisband.teamID );
            Interface.CallHook ( "OnTeamDisbanded", teamToDisband );
            Facepunch.Pool.Free ( ref teamToDisband );
            return false;
        }
    }
}