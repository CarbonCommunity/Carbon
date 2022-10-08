///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
	[CarbonHook("OnWorldPrefabSpawn"), CarbonHook.Category(Hook.Category.Enum.World)]
	[CarbonHook.Require("OnWorldPrefabSpawned")]
	[CarbonHook.Parameter("prefab", typeof(Prefab))]
	[CarbonHook.Parameter("position", typeof(Vector3))]
	[CarbonHook.Parameter("rotation", typeof(Quaternion))]
	[CarbonHook.Parameter("scale", typeof(Vector3))]
	[CarbonHook.Info("Called before a world prefab is spawned.")]
	[CarbonHook.Patch(typeof(World), "Spawn", true, typeof(string), typeof(Prefab), typeof(Vector3), typeof(Quaternion), typeof(Vector3))]
	public class World_Spawn_OnWorldPrefabSpawn
	{
		public static void Prefix(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale) { }
	}
}
