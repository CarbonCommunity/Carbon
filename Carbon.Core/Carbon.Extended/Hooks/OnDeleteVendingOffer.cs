using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RPC_DeleteSellOrder" )]
    public class OnDeleteVendingOffer
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnDeleteVendingOffer" );
        }
    }
}