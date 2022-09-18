using Carbon.Core;
using Harmony;
using Oxide.Core;
using ProtoBuf;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnTeamUpdate", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Team )]
    [Hook.Parameter ( "currentTeam", typeof ( ulong ) )]
    [Hook.Parameter ( "newTeam", typeof ( ulong ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when player's team is updated." )]
    [HarmonyPatch ( typeof ( BasePlayer ), "UpdateTeam" )]
    public class BasePlayer_UpdateTeam
    {
        public static bool Prefix ( ulong newTeam, ref BasePlayer __instance )
        {
            return Interface.CallHook("OnTeamUpdate", __instance.currentTeam, newTeam, __instance ) == null;
        }
    }
}