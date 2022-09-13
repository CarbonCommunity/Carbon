using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ElevatorLift ), "CanMove" )]
    public class CanElevatorLiftMove
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanElevatorLiftMove" );
        }
    }
}