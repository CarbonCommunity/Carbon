using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class Entity_BaseNetworkable
	{
		[HookAttribute.Patch("OnEntitySpawn", "OnEntitySpawn", typeof(BaseNetworkable), "Spawn", new System.Type[] { })]
		[HookAttribute.Identifier("c7d1643393324307bdaa4c11df129a66")]

		[MetadataAttribute.Info("Called before any networked entity has spawned (including trees).")]
		[MetadataAttribute.Parameter("networkable", typeof(BaseNetworkable))]

		public class Entity_BaseNetworkable_c7d1643393324307bdaa4c11df129a66 : Patch
		{
			public static void Prefix(ref BaseNetworkable __instance)
				=> HookCaller.CallStaticHook("OnEntitySpawn", __instance);
		}
	}
}
