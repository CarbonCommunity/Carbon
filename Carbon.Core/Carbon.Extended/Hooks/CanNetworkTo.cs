using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ShouldNetworkTo" )]
    public class CanNetworkTo
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanNetworkTo" );
        }
    }
}