using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( KeyLock ), "Lock" )]
    public class CanLock
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanLock" );
        }
    }
}