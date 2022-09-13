using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "DoTransaction" )]
    public class OnVendingTransaction
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVendingTransaction" );
        }
    }
}