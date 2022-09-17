using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;
using static BasePlayer;

namespace Carbon.Extended
{
    [Hook ( "OnPlayerSleepEnded", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "this", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the player awakes." )]
    [HarmonyPatch ( typeof ( BasePlayer ), "EndSleeping" )]
    public class BasePlayer_EndSleeping
    {
        public static bool Prefix ( ref BasePlayer __instance )
        {
            if ( !__instance.IsSleeping () )
            {
                return true;
            }

            return HookExecutor.CallStaticHook ( "OnPlayerSleepEnded", __instance ) == null;
        }
    }
}