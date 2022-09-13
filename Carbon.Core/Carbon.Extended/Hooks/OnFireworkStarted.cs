using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseFirework ), "Begin" )]
    public class OnFireworkStarted
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFireworkStarted" );
        }
    }
}