using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BearTrap ), "RPC_Arm" )]
    public class OnTrapArm
    {
        public static void Postfix ( BaseEntity.RPCMessage rpc , ref BearTrap __instance )
        {
            HookExecutor.CallStaticHook ( "OnTrapArm" );
        }
    }
}