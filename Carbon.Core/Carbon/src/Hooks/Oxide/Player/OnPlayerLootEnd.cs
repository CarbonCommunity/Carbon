///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Hooks
{
	[OxideHook("OnPlayerLootEnd", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("loot", typeof(PlayerLoot))]
	[OxideHook.Info("Called when the player stops looting.")]
	[OxideHook.Patch(typeof(PlayerLoot), "Clear")]
	public class PlayerLoot_Clear
	{
		public static bool Prefix(ref PlayerLoot __instance)
		{
			if (!__instance.IsLooting())
			{
				return false;
			}

			return HookCaller.CallStaticHook("OnPlayerLootEnd", __instance) == null;
		}
	}
}
