///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnPlayerSleep", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player is about to go to sleep.")]
	[OxideHook.Patch(typeof(BasePlayer), "StartSleeping")]
	public class BasePlayer_StartSleeping
	{
		public static bool Prefix(ref BasePlayer __instance)
		{
			if (__instance.IsSleeping())
			{
				return true;
			}

			return HookExecutor.CallStaticHook("OnPlayerSleep", __instance) == null;
		}
	}
}
