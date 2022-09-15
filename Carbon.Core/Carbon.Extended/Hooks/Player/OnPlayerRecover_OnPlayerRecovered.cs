using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [Hook ( "OnPlayerRecover" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "this", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the player is about to recover from the 'wounded' state." )]
    [HarmonyPatch ( typeof ( BasePlayer ), "RecoverFromWounded" )]
    public class BasePlayer_RecoverFromWounded_OnPlayerRecover
    {
        public static bool Prefix ( ref BasePlayer __instance )
        {
            return HookExecutor.CallStaticHook ( "OnPlayerRecover", __instance ) == null;
        }
    }

    [Hook ( "OnPlayerRecovered" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "this", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the player was recovered." )]
    [HarmonyPatch ( typeof ( BasePlayer ), "RecoverFromWounded" )]
    public class BasePlayer_RecoverFromWounded_OnPlayerRecovered
    {
        public static void Postfix ( ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerRecovered", __instance );
        }
    }
}