///
/// Copyright (c) 2022 bmgjet
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using Harmony;
using System.Reflection;
using UnityEngine;
using Network;

namespace Carbon.Core.Modules
{
	public class RustEditModule : CarbonModule<RustEditConfig, RustEditData>
	{
		public override string Name => "RustEdit.Ext";
		public override Type Type => typeof(RustEditModule);

		public List<Vector3> Spawnpoints = new List<Vector3>();

		public const string SpawnpointPrefab = "assets/bundled/prefabs/modding/volumes_and_triggers/spawn_point.prefab";

		[ChatCommand("showspawnpoints")]
		public void BroadcastSpawnPoints(BasePlayer player, string command, string[] args)
		{
			BroadcastSpawnpoints(player);
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
		public bool OnPlayerRespawn(BasePlayer __instance)
		{
			if (Spawnpoints == null || Spawnpoints.Count == 0) { return true; }

			var spawnpoint = Spawnpoints.GetRandom();
			var height = TerrainMeta.HeightMap.GetHeight(spawnpoint);

			if (spawnpoint.y <= height && AntiHack.TestInsideTerrain(spawnpoint)) { spawnpoint.y = height + 0.1f; }
			__instance.RespawnAt(spawnpoint, default);

			return false;
		}
		public bool SpawnHook(Vector3 position, Prefab prefab)
		{
			if (prefab.Name.Equals(SpawnpointPrefab))
			{
				Spawnpoints.Add(position);
				return false;
			}

			return true;
		}
	}

	public class RustEditConfig
	{
	}
	public class RustEditData
	{

	}
}

#region reference

/*
namespace Core.Spawn
{
	public class main
	{
		public static main codebase;
		public List<Vector3> Spawn_Points = new List<Vector3>();
		public void ShowSpawnPoints(BasePlayer player)
		{
			if (player.IsAdmin)
			{
				foreach (Vector3 v in Spawn_Points)
				{
					player.SendConsoleCommand("ddraw.sphere", 8f, Color.blue, v, 1f);
				}
			}
		}

		public bool sayAsHook(BasePlayer player, string message, ConVar.Chat.ChatChannel targetChannel)
		{
			if (message.ToLower() == "/showspawnpoints" || message.ToLower() == "\\showspawnpoints")
			{
				if (!player.IsAdmin) { return true; }
				player.ChatMessage("Showing spawn points");
				ShowSpawnPoints(player);
				return false;
			}
			return true;
		}

		public bool RespawnHook(BasePlayer __instance)
		{
			if (Spawn_Points == null || Spawn_Points.Count == 0) { return true; }
			Vector3 SpawnPoint = Spawn_Points.GetRandom<Vector3>();
			float TerrainHeight = TerrainMeta.HeightMap.GetHeight(SpawnPoint);
			if (SpawnPoint.y <= TerrainHeight && AntiHack.TestInsideTerrain(SpawnPoint)) { SpawnPoint.y = TerrainHeight + 0.1f; }
			__instance.RespawnAt(SpawnPoint, default);
			return false;
		}

		public bool InitializeHook()
		{
			if (Spawn_Points != null && Spawn_Points.Count != 0)
			{
				Debug.LogWarning("[Core.Spawn] Found " + Spawn_Points.Count + " Spawn Points");
			}
			return true;
		}

		public bool SpawnHook(Vector3 position, Prefab prefab)
		{
			if (prefab.Name == "assets/bundled/prefabs/modding/volumes_and_triggers/spawn_point.prefab")
			{
				Spawn_Points.Add(position);
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(World), "Spawn", typeof(string), typeof(Prefab), typeof(Vector3), typeof(Quaternion), typeof(Vector3))]
	internal class world_Spawn
	{
		[HarmonyPrefix]
		static bool Prefix(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			try
			{
				return main.codebase.SpawnHook(position, prefab);
			}
			catch { return true; }
		}
	}

	[HarmonyPatch(typeof(Server), "Initialize")]
	internal class Server_Initialize
	{
		[HarmonyPrefix]
		static bool Prefix()
		{
			try
			{
				return main.codebase.InitializeHook();
			}
			catch { }
			return true;
		}
	}

	[HarmonyPatch(typeof(BasePlayer), "Respawn")]
	internal class BasePlayer_Respawn
	{
		[HarmonyPrefix]
		static bool Prefix(BasePlayer __instance)
		{
			try
			{
				return main.codebase.RespawnHook(__instance);
			}
			catch { return true; }
		}
	}

	[HarmonyPatch(typeof(ConVar.Chat), "sayAs")]
	internal class Chat_sayAs
	{
		[HarmonyPrefix]
		static bool Prefix(ConVar.Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
		{
			try
			{
				return main.codebase.sayAsHook(player, message, targetChannel);
			}
			catch { return true; }
		}
	}
}
*/
#endregion
