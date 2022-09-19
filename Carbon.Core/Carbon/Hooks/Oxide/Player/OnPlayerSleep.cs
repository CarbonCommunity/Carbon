using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;
using static BasePlayer;

namespace Carbon.Extended
{
    [Hook ( "OnPlayerSleep", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "this", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the player is about to go to sleep." )]
    [Hook.Patch ( typeof ( BasePlayer ), "StartSleeping" )]
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