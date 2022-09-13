using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseVehicle ), "RPC_WantsPush" )]
    public class OnVehiclePush
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVehiclePush" );
        }
    }
}