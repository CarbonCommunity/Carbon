///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnHammerHit", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("hitInfo", typeof(HitInfo))]
	[OxideHook.Info("Called when the player has hit something with a hammer.")]
	[OxideHook.Patch(typeof(Hammer), "DoAttackShared")]
	public class Hammer_DoAttackShared
	{
		public static bool Prefix(HitInfo info, ref Hammer __instance)
		{
			return HookExecutor.CallStaticHook("OnHammerHit", __instance.GetOwnerPlayer(), info) == null;
		}
	}
}
