///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Hooks
{
	[CarbonHook("OnEntitySpawn"), CarbonHook.Category(Hook.Category.Enum.Entity)]
	[CarbonHook.Parameter("entity", typeof(BaseNetworkable))]
	[CarbonHook.Info("Called before any networked entity has spawned (including trees).")]
	[CarbonHook.Patch(typeof(BaseNetworkable), "Spawn")]
	public class BaseNetworkable_Spawn_OnEntitySpawn
	{
		public static void Prefix(ref BaseNetworkable __instance)
		{
			HookCaller.CallStaticHook("OnEntitySpawn", __instance);
		}
	}
}
