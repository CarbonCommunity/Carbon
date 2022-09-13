using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DeliveryDroneConfig ), "IsVendingMachineAccessible" )]
    public class CanAccessVendingMachine
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanAccessVendingMachine" );
        }
    }
}