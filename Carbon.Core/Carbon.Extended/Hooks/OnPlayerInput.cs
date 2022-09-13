using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnReceiveTick" )]
    public class OnPlayerInput
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerInput" );
        }
    }
}