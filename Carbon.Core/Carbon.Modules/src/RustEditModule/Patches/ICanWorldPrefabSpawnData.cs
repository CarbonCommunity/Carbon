using API.Hooks;
using ProtoBuf;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class RustEditModule
{
	[HookAttribute.Patch("ICanWorldPrefabSpawnData", "ICanWorldPrefabSpawnData", typeof(World), "Spawn", new System.Type[] { typeof(PrefabData) })]
	[HookAttribute.Identifier("4781d65c5a7d483ab393437ceeb0ea61")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class World_World_Spawn_4781d65c5a7d483ab393437ceeb0ea61 : API.Hooks.Patch
	{
		public static bool Prefix(PrefabData prefab)
		{
			return HookCaller.CallStaticHook(803331510, prefab) == null;
		}
	}
}
