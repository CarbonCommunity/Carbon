using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ModularCarGarage ), "RPC_SelectedLootItem" )]
    public class OnVehicleModuleSelect
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVehicleModuleSelect" );
        }
    }
}