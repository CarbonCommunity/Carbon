using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "CanOpenLootPanel" )]
    public class CanUseVending
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseVending" );
        }
    }
}