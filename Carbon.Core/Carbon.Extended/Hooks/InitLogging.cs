using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Bootstrap ), "StartupShared" )]
    public class InitLogging
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "InitLogging" );
        }
    }
}