using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "leaveteam" )]
    public class OnTeamLeave
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamLeave" );
        }
    }
}