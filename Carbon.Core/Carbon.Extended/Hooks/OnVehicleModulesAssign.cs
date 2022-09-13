using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ModularCar ), "SpawnPreassignedModules" )]
    public class OnVehicleModulesAssign
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVehicleModulesAssign" );
        }
    }
}