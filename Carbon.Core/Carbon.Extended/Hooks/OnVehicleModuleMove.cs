using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseModularVehicle ), "CanMoveFrom" )]
    public class OnVehicleModuleMove
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVehicleModuleMove" );
        }
    }
}