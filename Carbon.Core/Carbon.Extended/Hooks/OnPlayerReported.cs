using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnPlayerReported" )]
    public class OnPlayerReported
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerReported" );
        }
    }
}