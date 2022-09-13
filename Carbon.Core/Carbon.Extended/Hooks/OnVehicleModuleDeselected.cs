using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ModularCarGarage ), "RPC_DeselectedLootItem" )]
    public class OnVehicleModuleDeselected
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVehicleModuleDeselected" );
        }
    }
}