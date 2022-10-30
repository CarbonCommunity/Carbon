///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Hooks
{
	[OxideHook("OnPlayerSleepEnded", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player awakes.")]
	[OxideHook.Patch(typeof(BasePlayer), "EndSleeping")]
	public class BasePlayer_EndSleeping
	{
		public static bool Prefix(ref BasePlayer __instance)
		{
			if (!__instance.IsSleeping())
			{
				return true;
			}

			return HookCaller.CallStaticHook("OnPlayerSleepEnded", __instance) == null;
		}
	}
}
