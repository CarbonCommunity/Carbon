using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "sendinvite" )]
    public class OnTeamInvite
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamInvite" );
        }
    }
}