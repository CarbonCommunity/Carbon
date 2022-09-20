using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnTeamCreated" ), OxideHook.Category ( OxideHook.Category.Enum.Team )]
    [OxideHook.Require ( "OnTeamCreate" )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "team", typeof ( RelationshipManager.PlayerTeam ) )]
    [OxideHook.Info ( "Called after a team was created." )]
    [OxideHook.Patch ( typeof ( RelationshipManager ), "trycreateteam" )]
    public class RelationshipManager_trycreateteam_OnTeamCreated
    {
        public static void Postfix ( ConsoleSystem.Arg arg )
        {

        }
    }
}