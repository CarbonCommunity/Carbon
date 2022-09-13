using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RPC_RotateVM" )]
    public class OnRotateVendingMachine
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRotateVendingMachine" );
        }
    }
}