using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "AddSellOrder" )]
    public class OnAddVendingOffer
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnAddVendingOffer" );
        }
    }
}