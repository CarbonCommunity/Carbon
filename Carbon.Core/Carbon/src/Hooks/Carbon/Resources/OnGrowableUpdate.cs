///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[CarbonHook("OnGrowableUpdate"), CarbonHook.Category(CarbonHook.Category.Enum.Resources)]
	[CarbonHook.Parameter("this", typeof(GrowableEntity))]
	[CarbonHook.Info("Called before growable entity will be updated.")]
	[CarbonHook.Patch(typeof(GrowableEntity), "RunUpdate")]
	public class GrowableEntity_RunUpdate
	{
		public static void Prefix(ref GrowableEntity __instance)
		{
			HookCaller.CallStaticHook("OnGrowableUpdate", __instance);
		}
	}
}
