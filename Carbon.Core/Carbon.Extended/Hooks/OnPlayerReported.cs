using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnPlayerReported" )]
    public class OnPlayerReported
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerReported" );
        }
    }
}