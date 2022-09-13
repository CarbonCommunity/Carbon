using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BearTrap ), "RPC_Arm" )]
    public class OnTrapArm
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTrapArm" );
        }
    }
}