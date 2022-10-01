///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnPlayerSleepEnded", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player awakes.")]
	[OxideHook.Patch(typeof(BasePlayer), "EndSleeping")]
	public class BasePlayer_EndSleeping
	{
		public static bool Prefix (ref BasePlayer __instance)
		{
			if (!__instance.IsSleeping())
			{
				return true;
			}

			return HookExecutor.CallStaticHook("OnPlayerSleepEnded", __instance) == null;
		}
	}
}
