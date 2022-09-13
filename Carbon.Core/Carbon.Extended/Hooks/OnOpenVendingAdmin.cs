using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RPC_OpenAdmin" )]
    public class OnOpenVendingAdmin
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnOpenVendingAdmin" );
        }
    }
}