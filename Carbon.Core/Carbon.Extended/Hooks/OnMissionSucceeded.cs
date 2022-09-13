using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMission ), "MissionSuccess" )]
    public class OnMissionSucceeded
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMissionSucceeded" );
        }
    }
}