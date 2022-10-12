///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnWorldPrefabSpawned"), OxideHook.Category(Hook.Category.Enum.World)]
	[OxideHook.Parameter("gameObject", typeof(GameObject))]
	[OxideHook.Parameter("category", typeof(string))]
	[OxideHook.Info("Called when a world prefab has been spawned.")]
	[OxideHook.Patch(typeof(World), "Spawn", true, typeof(string), typeof(Prefab), typeof(Vector3), typeof(Quaternion), typeof(Vector3))]
	public class World_Spawn
	{
		public static bool Prefix(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			if (Interface.CallHook("OnWorldPrefabSpawn", prefab, position, rotation, scale) != null)
			{
				return false;
			}

			if (prefab == null || !prefab.Object)
			{
				return false;
			}

			if (!World.Cached)
			{
				prefab.ApplyTerrainPlacements(position, rotation, scale);
				prefab.ApplyTerrainModifiers(position, rotation, scale);
			}

			var gameObject = prefab.Spawn(position, rotation, scale, true);

			if (gameObject)
			{
				Interface.CallHook("OnWorldPrefabSpawned", gameObject, category);
				gameObject.SetHierarchyGroup(category, true, false);
			}

			return false;
		}
	}
}
