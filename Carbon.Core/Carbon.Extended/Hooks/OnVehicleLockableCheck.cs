using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ModularCarLock ), "CanHaveALock" )]
    public class OnVehicleLockableCheck
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnVehicleLockableCheck" );
        }
    }
}