using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "TeamUpdate" )]
    public class OnTeamUpdated
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamUpdated" );
        }
    }
}