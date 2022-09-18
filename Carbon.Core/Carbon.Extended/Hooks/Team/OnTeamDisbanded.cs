using Carbon.Core;
using ConVar;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamDisbanded"), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Info ( "Called when the team was disbanded." )]
    [HarmonyPatch ( typeof ( RelationshipManager ), "DisbandTeam" )]
    public class RelationshipManager_DisbandTeam_OnTeamDisbanded
    {
        public static void Postfix ( RelationshipManager.PlayerTeam teamToDisband )
        {
        }
    }
}