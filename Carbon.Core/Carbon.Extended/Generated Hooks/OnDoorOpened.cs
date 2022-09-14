using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Door ), "RPC_OpenDoor" )]
    public class OnDoorOpened
    {
        public static void Postfix ( BaseEntity.RPCMessage rpc , ref Door __instance )
        {
            HookExecutor.CallStaticHook ( "OnDoorOpened" );
        }
    }
}