using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "OnTryToOpen" )]
    public class CanUseLockedEntity [KeyLock, open]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseLockedEntity [KeyLock, open]" );
        }
    }
}