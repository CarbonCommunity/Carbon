using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;
using static BasePlayer;

namespace Carbon.Extended
{
    [Hook ( "OnPlayerRespawned" ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "this", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when the player has respawned (specifically when they click the \"Respawn\" button)." )]
    [Hook.Info ( "ONLY called after the player has transitioned from dead to not-dead, so not when they're waking up." )]
    [Hook.Info ( "This means it's possible for the player to connect and disconnect from a server without OnPlayerRespawned ever triggering for them." )]
    [Hook.Patch ( typeof ( BasePlayer ), "RespawnAt" )]
    public class BasePlayer_Respawned
    {
        public static void Postfix ( Vector3 position, Quaternion rotation, ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerRespawned", __instance );
        }
    }
}