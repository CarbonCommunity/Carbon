using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Door ), "RPC_CloseDoor" )]
    public class OnDoorClosed
    {
        public static void Postfix ( BaseEntity.RPCMessage rpc , ref Door __instance )
        {
            HookExecutor.CallStaticHook ( "OnDoorClosed" );
        }
    }
}