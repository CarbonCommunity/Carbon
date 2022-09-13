using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMission ), "MissionFailed" )]
    public class OnMissionFailed
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMissionFailed" );
        }
    }
}