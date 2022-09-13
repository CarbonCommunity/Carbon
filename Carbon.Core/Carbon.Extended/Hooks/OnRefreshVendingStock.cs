using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RefreshSellOrderStockLevel" )]
    public class OnRefreshVendingStock
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRefreshVendingStock" );
        }
    }
}