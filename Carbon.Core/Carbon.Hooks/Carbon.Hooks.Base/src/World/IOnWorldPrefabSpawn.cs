using System.Threading;
using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_World
{
	public partial class World_CargoShip
	{
		[HookAttribute.Patch("IOnWorldPrefabSpawn", "IOnWorldPrefabSpawn", typeof(World), "Spawn", new System.Type[] { typeof(System.String), typeof(Prefab), typeof(UnityEngine.Vector3), typeof(UnityEngine.Quaternion), typeof(UnityEngine.Vector3) })]
		[HookAttribute.Identifier("37656a8fcdb7486693f0085c831f11ad")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class World_CargoShip_StartEgress_37656a8fcdb7486693f0085c831f11ad : API.Hooks.Patch
		{
			public static bool Prefix(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
			{
				return HookCaller.CallStaticHook("IOnWorldPrefabSpawn", category, prefab, position, rotation, scale) == null;
			}
		}
	}
}
