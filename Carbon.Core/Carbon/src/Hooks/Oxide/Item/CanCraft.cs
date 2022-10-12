///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanCraft", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("this", typeof(ItemCrafter))]
	[OxideHook.Parameter("blueprint", typeof(ItemBlueprint))]
	[OxideHook.Parameter("amount", typeof(int))]
	[OxideHook.Parameter("free", typeof(bool))]
	[OxideHook.Info("Called when the player attempts to craft an item.")]
	[OxideHook.Patch(typeof(ItemCrafter), "CanCraft", typeof(ItemBlueprint), typeof(int?), typeof(bool?))]
	public class ItemCrafter_CanCraft
	{
		public static bool Prefix(ItemBlueprint bp, int amount, bool free, out bool __result, ref ItemCrafter __instance)
		{
			if (HookExecutor.CallStaticHook("CanCraft", __instance, bp, amount, free) is bool flag)
			{
				__result = flag;
				return false;
			}

			__result = default;
			return true;
		}
	}
}
