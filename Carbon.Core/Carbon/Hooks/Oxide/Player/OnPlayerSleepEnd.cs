using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;
using static BasePlayer;

namespace Carbon.Extended
{
    [OxideHook ( "OnPlayerSleepEnded", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "this", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Called when the player awakes." )]
    [OxideHook.Patch ( typeof ( BasePlayer ), "EndSleeping" )]
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