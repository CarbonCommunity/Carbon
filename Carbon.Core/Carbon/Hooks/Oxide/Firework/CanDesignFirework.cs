///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("CanDesignFirework", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Firework)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(PatternFirework))]
	[OxideHook.Info("Useful for blocking players from editing pattern fireworks.")]
	[OxideHook.Patch(typeof(PatternFirework), "PlayerCanModify")]
	public class PatternFirework_PlayerCanModify
	{
		public static bool Prefix (BasePlayer player, ref PatternFirework __instance, ref bool __result)
		{
			if (player == null || !player.CanInteract())
			{
				__result = false;

				return false;
			}

			object returnvar = HookExecutor.CallStaticHook("CanDesignFirework", player, __instance);

			if (returnvar is bool value)
			{
				__result = value;

				return false;
			}

			return true;
		}
	}
}
