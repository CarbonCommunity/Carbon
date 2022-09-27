///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Extended
{
    [OxideHook ( "OnTeamCreated" ), OxideHook.Category ( Hook.Category.Enum.Team )]
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