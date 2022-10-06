

using Carbon.Core;


///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Extended
{
	[OxideHook("CanCraft"), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("this", typeof(ItemCrafter))]
	[OxideHook.Parameter("bp", typeof(ItemBlueprint))]
	[OxideHook.Parameter("amount", typeof(int))]
	[OxideHook.Parameter("free", typeof(bool))]
	[OxideHook.Info("Called when the player attempts to craft an item.")]
	[OxideHook.Patch(typeof(ItemCrafter), "CanCraft")]
	public class ItemCrafter_CanCraft
	{
		public static bool Prefix(ItemBlueprint bp, int amount, bool free, ref ItemCrafter __instance)
		{
			if (HookExecutor.CallStaticHook("CanCraft", __instance, bp, amount, free) is bool flag)
				return flag;
			return true;
		}
	}
}
