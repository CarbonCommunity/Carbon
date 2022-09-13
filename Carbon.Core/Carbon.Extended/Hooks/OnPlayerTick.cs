using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnReceiveTick" )]
    public class OnPlayerTick
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerTick" );
        }
    }
}