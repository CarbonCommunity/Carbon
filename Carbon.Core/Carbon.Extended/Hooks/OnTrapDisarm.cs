using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Landmine ), "RPC_Disarm" )]
    public class OnTrapDisarm
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTrapDisarm" );
        }
    }
}