///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
namespace Carbon.Hooks
{
	[OxideHook("OnBasePlayerAttacked", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Parameter("hitInfo", typeof(HitInfo))]
	[OxideHook.Info("Called before the player is being attacked.")]
	[OxideHook.Patch(typeof(BasePlayer), "OnAttacked")]
	public class BasePlayer_OnAttacked
	{
		public static bool Prefix(HitInfo info, ref BasePlayer __instance)
		{
			return HookCaller.CallStaticHook("OnPlayerSleep", __instance, info) == null;
		}
	}
}
