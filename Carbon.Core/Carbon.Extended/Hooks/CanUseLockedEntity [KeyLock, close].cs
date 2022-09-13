using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "OnTryToClose" )]
    public class CanUseLockedEntity [KeyLock, close]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseLockedEntity [KeyLock, close]" );
        }
    }
}