///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnIngredientsCollect", typeof(object)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("this", typeof(ItemCrafter))]
	[OxideHook.Parameter("bp", typeof(ItemBlueprint))]
	[OxideHook.Parameter("task", typeof(ItemCraftTask))]
	[OxideHook.Parameter("amount", typeof(int))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when ingredients are about to be collected for crafting an item.")]
	[OxideHook.Patch(typeof(ItemCrafter), "CollectIngredients")]
	public class ItemCrafter_CollectIngredients
	{
		public static bool Prefix(ItemBlueprint bp, ItemCraftTask task, int amount, BasePlayer player, ref ItemCrafter __instance)
		{
			return HookExecutor.CallStaticHook("OnIngredientsCollect", __instance, bp, task, amount, player) == null;
		}
	}
}
