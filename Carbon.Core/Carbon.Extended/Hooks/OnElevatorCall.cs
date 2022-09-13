using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Elevator ), "<CallElevator>b__28_0" )]
    public class OnElevatorCall
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnElevatorCall" );
        }
    }
}