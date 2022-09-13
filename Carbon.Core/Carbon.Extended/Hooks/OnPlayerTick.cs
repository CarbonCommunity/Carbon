using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnReceiveTick" )]
    public class OnPlayerTick
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerTick" );
        }
    }
}