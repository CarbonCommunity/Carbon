using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RelationshipManager ), "trycreateteam" )]
    public class OnTeamCreate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamCreate" );
        }
    }
}