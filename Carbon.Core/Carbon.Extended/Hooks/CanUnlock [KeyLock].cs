using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "RPC_Unlock" )]
    public class CanUnlock [KeyLock]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUnlock [KeyLock]" );
        }
    }
}