using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;
using static BasePlayer;

namespace Carbon.Extended
{
    [OxideHook ( "OnPlayerSleep", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "this", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Called when the player is about to go to sleep." )]
    [OxideHook.Patch ( typeof ( BasePlayer ), "StartSleeping" )]
    public class BasePlayer_StartSleeping
    {
        public static bool Prefix ( ref BasePlayer __instance )
        {
            if ( __instance.IsSleeping () )
            {
                return true;
            }

            return HookExecutor.CallStaticHook ( "OnPlayerSleep", __instance ) == null;
        }
    }
}