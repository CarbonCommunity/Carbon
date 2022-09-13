using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "trycreateteam" )]
    public class OnTeamCreated
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamCreated" );
        }
    }
}