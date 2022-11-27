/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class Entity_BaseNetworkable
	{
		/*
		[CarbonHook("OnEntitySpawn"), CarbonHook.Category(Hook.Category.Enum.Entity)]
		[CarbonHook.Parameter("entity", typeof(BaseNetworkable))]
		[CarbonHook.Info("Called before any networked entity has spawned (including trees).")]
		[CarbonHook.Patch(typeof(BaseNetworkable), "Spawn")]
		*/

		public class Entity_BaseNetworkable_Spawn_c7d1643393324307bdaa4c11df129a66
		{
			public static Metadata metadata = new Metadata("OnEntitySpawn",
				typeof(BaseNetworkable), "Spawn", new System.Type[] { });

			static Entity_BaseNetworkable_Spawn_c7d1643393324307bdaa4c11df129a66()
			{
				metadata.SetIdentifier("c7d1643393324307bdaa4c11df129a66");
			}

			public static void Prefix(ref BaseNetworkable __instance)
			{
				HookCaller.CallStaticHook("OnEntitySpawn", __instance);
			}
		}
	}
}