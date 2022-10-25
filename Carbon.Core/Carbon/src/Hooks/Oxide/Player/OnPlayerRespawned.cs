///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnPlayerRespawned"), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player has respawned (specifically when they click the \"Respawn\" button).")]
	[OxideHook.Info("ONLY called after the player has transitioned from dead to not-dead, so not when they're waking up.")]
	[OxideHook.Info("This means it's possible for the player to connect and disconnect from a server without OnPlayerRespawned ever triggering for them.")]
	[OxideHook.Patch(typeof(BasePlayer), "RespawnAt")]
	public class BasePlayer_Respawned
	{
		public static void Postfix(Vector3 position, Quaternion rotation, ref BasePlayer __instance)
		{
			HookCaller.CallStaticHook("OnPlayerRespawned", __instance);
		}
	}
}
