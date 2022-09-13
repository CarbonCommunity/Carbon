using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "DisbandTeam" )]
    public class OnTeamDisband
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamDisband" );
        }
    }
}