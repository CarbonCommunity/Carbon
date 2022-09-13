using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "rejectinvite" )]
    public class OnTeamRejectInvite
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamRejectInvite" );
        }
    }
}