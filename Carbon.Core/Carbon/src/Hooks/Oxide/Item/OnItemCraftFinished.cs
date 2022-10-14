///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnItemCraftFinished"), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("task", typeof(ItemCraftTask))]
	[OxideHook.Parameter("byItemId", typeof(Item))]
	[OxideHook.Info("Called right after an item has been crafted.")]
	[OxideHook.Patch(typeof(ItemCrafter), "FinishCrafting")]
	public class ItemCrafter_FinishCrafting
	{
		public static void Prefix(ItemCraftTask task)
		{
			var skin = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
			var byItemId = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, skin: skin);
			byItemId.amount = task.blueprint.amountToCreate;
			if (byItemId.hasCondition && (double)task.conditionScale != 1.0)
			{
				byItemId.maxCondition *= task.conditionScale;
				byItemId.condition = byItemId.maxCondition;
			}
			byItemId.OnVirginSpawn();

			HookCaller.CallStaticHook("OnItemCraftFinished", task, byItemId);
		}
	}
}
