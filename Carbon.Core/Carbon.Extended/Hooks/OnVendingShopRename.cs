using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RPC_UpdateShopName" )]
    public class OnVendingShopRename
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnVendingShopRename" );
        }
    }
}