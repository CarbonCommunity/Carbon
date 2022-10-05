///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[Hook("OnPlayerRecover", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player is about to recover from the 'wounded' state.")]
	[OxideHook.Patch(typeof(BasePlayer), "RecoverFromWounded")]
	public class BasePlayer_RecoverFromWounded_OnPlayerRecover
	{
		public static bool Prefix(ref BasePlayer __instance)
		{
			return HookExecutor.CallStaticHook("OnPlayerRecover", __instance) == null;
		}
	}

	[OxideHook("OnPlayerRecovered"), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player was recovered.")]
	[OxideHook.Patch(typeof(BasePlayer), "RecoverFromWounded")]
	public class BasePlayer_RecoverFromWounded_OnPlayerRecovered
	{
		public static void Postfix(ref BasePlayer __instance)
		{
			HookExecutor.CallStaticHook("OnPlayerRecovered", __instance);
		}
	}
}
