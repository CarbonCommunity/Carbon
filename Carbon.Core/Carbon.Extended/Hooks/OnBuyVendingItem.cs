using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "BuyItem" )]
    public class OnBuyVendingItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnBuyVendingItem" );
        }
    }
}