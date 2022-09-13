using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "trycreateteam" )]
    public class OnTeamCreate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamCreate" );
        }
    }
}