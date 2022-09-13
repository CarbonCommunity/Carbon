using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "DisbandTeam" )]
    public class OnTeamDisbanded
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamDisbanded" );
        }
    }
}