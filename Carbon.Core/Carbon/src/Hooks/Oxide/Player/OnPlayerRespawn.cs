///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Linq;
using Carbon.Core;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnPlayerRespawn", typeof(BasePlayer.SpawnPoint)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(BasePlayer.SpawnPoint))]
	[OxideHook.Info("Called when a player is attempting to respawn.")]
	[OxideHook.Info("Returning a BasePlayer.SpawnPoint (takes a position and rotation) overrides the respawn location.")]
	[OxideHook.Info("Returning a SleepingBag overrides the respawn location.")]
	[OxideHook.Patch(typeof(BasePlayer), "Respawn")]
	public class BasePlayer_Respawn
	{
		public static bool Prefix(ref BasePlayer __instance)
		{
			var spawnPoint = (BasePlayer.SpawnPoint)null;
			var obj = HookExecutor.CallStaticHook("OnPlayerRespawn", __instance, spawnPoint);

			if (obj is BasePlayer.SpawnPoint point) spawnPoint = point; else spawnPoint = ServerMgr.FindSpawnPoint(__instance);

			__instance.RespawnAt(spawnPoint.pos, spawnPoint.rot);
			return false;
		}
	}

	[OxideHook("OnPlayerRespawn", typeof(SleepingBag)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(SleepingBag))]
	[OxideHook.Info("Called when a player is attempting to respawn.")]
	[OxideHook.Info("Returning a SleepingBag overrides the respawn location.")]
	[OxideHook.Patch(typeof(SleepingBag), "SpawnPlayer")]
	public class SleepingBag_SpawnPlayer
	{
		public static bool Prefix(BasePlayer player, uint sleepingBag, out bool __result)
		{
			var array = SleepingBag.FindForPlayer(player.userID, true);
			var sleepingBag2 = Enumerable.FirstOrDefault(array, (SleepingBag x) => x.ValidForPlayer(player.userID, false) && x.net.ID == sleepingBag && x.unlockTime < UnityEngine.Time.realtimeSinceStartup);
			if (sleepingBag2 == null)
			{
				__result = false;
				return false;
			}

			var obj = Interface.CallHook("OnPlayerRespawn", player, sleepingBag2);

			if (obj is SleepingBag)
			{
				sleepingBag2 = (SleepingBag)obj;
			}

			if (sleepingBag2.IsOccupied())
			{
				__result = false;
				return false;
			}

			sleepingBag2.GetSpawnPos(out var position, out var rotation);
			player.RespawnAt(position, rotation);
			sleepingBag2.PostPlayerSpawn(player);

			for (int i = 0; i < array.Length; i++)
			{
				SleepingBag.SetBagTimer(array[i], position, SleepingBag.SleepingBagResetReason.Respawned);
			}

			__result = true;
			return false;
		}
	}
}
