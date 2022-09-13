using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Locker ), "RPC_Equip" )]
    public class OnLockerSwap
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLockerSwap" );
        }
    }
}