///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Oxide.Core;
using UnityEngine;
using static BaseCombatEntity;

namespace Carbon.Extended
{
	[OxideHook("OnPlayerDeath", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("info", typeof(HitInfo))]
	[OxideHook.Info("Called when the player is about to die.")]
	[OxideHook.Info("HitInfo may be null sometimes.")]
	[OxideHook.Patch(typeof(BasePlayer), "Die")]
	public class BasePlayer_Die
	{
		public static bool Prefix(HitInfo info, ref BasePlayer __instance)
		{
			try
			{
				if (!__instance.IsDead())
				{
					if (__instance.Belt != null && __instance.ShouldDropActiveItem())
					{
						var vector = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0.2f, UnityEngine.Random.Range(-2f, 2f));
						__instance.Belt.DropActive(__instance.GetDropPosition(), __instance.GetInheritedDropVelocity() + vector.normalized * 3f);
					}
					if (!__instance.WoundInsteadOfDying(info))
					{
						if (Interface.CallHook("OnPlayerDeath", __instance, info) != null)
						{
							return false;
						}
						SleepingBag.OnPlayerDeath(__instance);
						BaseDie(__instance, info);
					}
				}

				return false;
			}
			catch (Exception ex) { Logger.Instance.Error($"Failed OnPlayerDeath", ex); }

			return false;
		}

		public static void BaseDie(BasePlayer player, HitInfo info)
		{
			if (player.IsDead())
			{
				return;
			}

			if (ConVar.Global.developer > 1)
			{
				Debug.Log("[Combat]".PadRight(10) + player.gameObject.name + " died");
			}

			player.health = 0f;
			player.lifestate = LifeState.Dead;
			if (info != null && (bool)info.InitiatorPlayer)
			{
				BasePlayer initiatorPlayer = info.InitiatorPlayer;
				if (initiatorPlayer != null && initiatorPlayer.GetActiveMission() != -1 && !initiatorPlayer.IsNpc)
				{
					initiatorPlayer.ProcessMissionEvent(BaseMission.MissionEventType.KILL_ENTITY, player.prefabID.ToString(), 1f);
				}
			}

			player.OnKilled(info);
		}
	}
}
