using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CodeLock ), "OnTryToClose" )]
    public class CanUseLockedEntity [CodeLock, close]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseLockedEntity [CodeLock, close]" );
        }
    }
}