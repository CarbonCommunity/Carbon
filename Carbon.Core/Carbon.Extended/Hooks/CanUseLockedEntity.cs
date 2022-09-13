using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "OnTryToOpen" )]
    public class CanUseLockedEntity
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseLockedEntity" );
        }
    }
}