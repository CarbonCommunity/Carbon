///
/// Copyright (c) 2022 bmgjet
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using Harmony;
using Network;
using UnityEngine;
using static BasePlayer;

namespace Carbon.Core.Modules
{
	public class RustEditModule : CarbonModule<RustEditConfig, RustEditData>
	{
		public override string Name => "RustEdit.Ext";
		public override Type Type => typeof(RustEditModule);

		public List<Vector3> Spawnpoints = new List<Vector3>();

		public const string SpawnpointPrefab = "assets/bundled/prefabs/modding/volumes_and_triggers/spawn_point.prefab";

		[ChatCommand("showspawnpoints")]
		public void DoBroadcastSpawnpoints(BasePlayer player)
		{
			if (!ConfigInstance.Enabled) return;

			BroadcastSpawnpoints(player);
		}

		private void OnServerInitialized()
		{
			if (!ConfigInstance.Enabled) return;

			if (Spawnpoints != null && Spawnpoints.Count != 0)
			{
				PutsWarn($"Found {Spawnpoints.Count:n0} spawn-points.");
			}
		}

		private object OnPlayerRespawn(BasePlayer player, BasePlayer.SpawnPoint point)
		{
			if (!ConfigInstance.Enabled) return null;

			return RespawnPlayer();
		}
		private object OnWorldPrefabSpawn(Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			if (!ConfigInstance.Enabled) return null;

			if (prefab.Name.Equals(SpawnpointPrefab))
			{
				Spawnpoints.Add(position);
				return true;
			}

			return null;
		}

		public SpawnPoint RespawnPlayer()
		{
			if (Spawnpoints == null || Spawnpoints.Count == 0) { return null; }

			var spawnpoint = Spawnpoints.GetRandom();
			var height = TerrainMeta.HeightMap.GetHeight(spawnpoint);

			if (spawnpoint.y <= height && AntiHack.TestInsideTerrain(spawnpoint)) { spawnpoint.y = height + 0.1f; }

			return new SpawnPoint { pos = spawnpoint, rot = default };
		}

		public void BroadcastSpawnpoints(BasePlayer player)
		{
			if (!player.IsAdmin) return;

			player.ChatMessage("Showing spawn points");

			foreach (var spawnpoint in Spawnpoints)
			{
				player.SendConsoleCommand("ddraw.sphere", 8f, Color.blue, spawnpoint, 1f);
			}
		}
	}

	public class RustEditConfig
	{
	}
	public class RustEditData
	{

	}
}
