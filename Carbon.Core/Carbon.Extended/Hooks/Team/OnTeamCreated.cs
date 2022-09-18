using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamCreated" ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Require ( "OnTeamCreate" )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [Hook.Info ( "Called after a team was created." )]
    [HarmonyPatch ( typeof ( RelationshipManager ), "trycreateteam" )]
    public class RelationshipManager_trycreateteam_OnTeamCreated
    {
        public static void Postfix ( ConsoleSystem.Arg arg )
        {

        }
    }
}