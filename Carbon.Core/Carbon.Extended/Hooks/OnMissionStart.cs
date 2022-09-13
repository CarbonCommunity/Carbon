using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMission ), "MissionStart" )]
    public class OnMissionStart
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMissionStart" );
        }
    }
}