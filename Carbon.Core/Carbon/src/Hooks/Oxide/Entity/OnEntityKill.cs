///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnEntityKill", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(BaseNetworkable))]
	[OxideHook.Info("Called when an entity is destroyed.")]
	[OxideHook.Patch(typeof(BaseNetworkable), "Kill")]
	public class BaseNetworkable_Kill
	{
		public static bool Prefix(BaseNetworkable.DestroyMode mode, ref BaseNetworkable __instance)
		{
			if (__instance.IsDestroyed)
			{
				return true;
			}

			return HookCaller.CallStaticHook("OnEntityKill", __instance) == null;
		}
	}
}
