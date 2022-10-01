///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("CanLootPlayer"), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("looter", typeof(BasePlayer))]
	[OxideHook.Parameter("target", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player attempts to loot another player.")]
	[OxideHook.Patch(typeof(BasePlayer), "CanBeLooted")]
	public class BasePlayer_CanBeLooted
	{
		public static bool Prefix (BasePlayer player, ref BasePlayer __instance, out bool __result)
		{
			var obj = Interface.CallHook("CanLootPlayer", __instance, player);

			if (obj is bool)
			{
				__result = (bool)obj;
				return false;
			}

			__result = false;
			return true;
		}
	}
}
