using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "CanAcceptItem" )]
    public class CanVendingAcceptItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanVendingAcceptItem" );
        }
    }
}