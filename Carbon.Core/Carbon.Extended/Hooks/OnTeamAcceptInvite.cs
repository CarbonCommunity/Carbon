using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "acceptinvite" )]
    public class OnTeamAcceptInvite
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamAcceptInvite" );
        }
    }
}