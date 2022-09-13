using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "RPC_Broadcast" )]
    public class OnToggleVendingBroadcast
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnToggleVendingBroadcast" );
        }
    }
}