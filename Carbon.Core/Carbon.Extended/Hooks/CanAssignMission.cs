using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMission ), "AssignMission" )]
    public class CanAssignMission
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanAssignMission" );
        }
    }
}