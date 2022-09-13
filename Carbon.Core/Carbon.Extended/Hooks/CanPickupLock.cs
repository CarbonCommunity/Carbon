using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseLock ), "RPC_TakeLock" )]
    public class CanPickupLock
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanPickupLock" );
        }
    }
}