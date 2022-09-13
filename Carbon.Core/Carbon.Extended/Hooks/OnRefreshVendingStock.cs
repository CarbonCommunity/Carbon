using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RefreshSellOrderStockLevel" )]
    public class OnRefreshVendingStock
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRefreshVendingStock" );
        }
    }
}