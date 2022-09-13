using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "RPC_Unlock" )]
    public class CanUnlock
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanUnlock" );
        }
    }
}