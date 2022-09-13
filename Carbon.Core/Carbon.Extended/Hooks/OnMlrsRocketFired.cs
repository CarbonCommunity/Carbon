using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MLRS ), "FireNextRocket" )]
    public class OnMlrsRocketFired
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMlrsRocketFired" );
        }
    }
}