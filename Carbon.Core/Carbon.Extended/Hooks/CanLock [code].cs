using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CodeLock ), "TryLock" )]
    public class CanLock [code]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLock [code]" );
        }
    }
}