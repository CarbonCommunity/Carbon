using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnReceiveTick" )]
    public class OnPlayerTick
    {
        public static void Postfix ( PlayerTick msg, System.Boolean wasPlayerStalled )
        {
            HookExecutor.CallStaticHook ( "OnPlayerTick" );
        }
    }
}