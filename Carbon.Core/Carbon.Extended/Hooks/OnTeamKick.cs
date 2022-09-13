using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "kickmember" )]
    public class OnTeamKick
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamKick" );
        }
    }
}