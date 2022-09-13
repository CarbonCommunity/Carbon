using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CodeLock ), "OnTryToOpen" )]
    public class CanUseLockedEntity [CodeLock, open]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseLockedEntity [CodeLock, open]" );
        }
    }
}