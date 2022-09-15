using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "RecoverFromWounded" )]
    public class BasePlayer_RecoverFromWounded
    {
        public static bool Prefix ( ref BasePlayer __instance )
        {
            return HookExecutor.CallStaticHook ( "OnPlayerRecover", __instance ) == null;
        }

        public static void Postfix ( ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerRecovered", __instance );
        }
    }
}