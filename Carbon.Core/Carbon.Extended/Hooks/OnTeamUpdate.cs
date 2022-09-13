using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "UpdateTeam" )]
    public class OnTeamUpdate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTeamUpdate" );
        }
    }
}