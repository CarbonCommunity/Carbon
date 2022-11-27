using UnityEngine;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_World
{
	public partial class World_World
	{
		/*
		[CarbonHook("OnWorldPrefabSpawn"), CarbonHook.Category(Hook.Category.Enum.World)]
		[CarbonHook.Require("OnWorldPrefabSpawned")]
		[CarbonHook.Parameter("prefab", typeof(Prefab))]
		[CarbonHook.Parameter("position", typeof(Vector3))]
		[CarbonHook.Parameter("rotation", typeof(Quaternion))]
		[CarbonHook.Parameter("scale", typeof(Vector3))]
		[CarbonHook.Info("Called before a world prefab is spawned.")]
		[CarbonHook.Patch(typeof(World), "Spawn", true, typeof(string), typeof(Prefab), typeof(Vector3), typeof(Quaternion), typeof(Vector3))]
		*/

		public class World_World_Spawn_1986e3235a1946149d45132ed5ba3f0a
		{
			public static Metadata metadata = new Metadata("OnWorldPrefabSpawn",
				typeof(World), "Spawn", new System.Type[] { typeof(string), typeof(Prefab), typeof(Vector3), typeof(Quaternion), typeof(Vector3) });

			static World_World_Spawn_1986e3235a1946149d45132ed5ba3f0a()
			{
				metadata.SetIdentifier("1986e3235a1946149d45132ed5ba3f0a");
			}

			public static void Prefix(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale) { }
		}
	}
}