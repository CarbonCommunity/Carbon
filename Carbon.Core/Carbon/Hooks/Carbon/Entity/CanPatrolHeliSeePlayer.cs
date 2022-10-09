///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Core.Hooks.Carbon.Entity
{
	[CarbonHook("CanPatrolHeliSeePlayer", typeof(bool)), CarbonHook.Category(Hook.Category.Enum.Entity)]
	[CarbonHook.Parameter("this", typeof(PatrolHelicopterAI))]
	[CarbonHook.Parameter("player", typeof(BasePlayer))]
	[CarbonHook.Info("Can the Patrol Helicopter see the player.")]
	[CarbonHook.Patch(typeof(PatrolHelicopterAI), "PlayerVisible")]
	public class PatrolHelicopterAI_PlayerVisible
	{
		public static bool Prefix(BasePlayer ply, ref PatrolHelicopterAI __instance, out bool __result)
		{
			var obj = Interface.CallHook("CanPatrolHeliSeePlayer", __instance, ply);
			if (obj is bool)
			{
				__result = (bool)obj;
				return false;
			}

			__result = default;
			return true;
		}
	}
}
