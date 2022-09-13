using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RPC_OpenShop" )]
    public class OnVendingShopOpen [VendingMachine]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnVendingShopOpen [VendingMachine]" );
        }
    }
}