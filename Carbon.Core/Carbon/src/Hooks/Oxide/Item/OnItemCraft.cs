///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Hooks
{
	[OxideHook("OnItemCraft", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("task", typeof(ItemCraftTask))]
	[OxideHook.Parameter("owner", typeof(BasePlayer))]
	[OxideHook.Parameter("fromTempBlueprint", typeof(Item))]
	[OxideHook.Info("Called right after an item is added to the crafting queue")]
	[OxideHook.Patch(typeof(ItemCrafter), "CraftItem")]
	public class ItemCrafter_CraftItem
	{
		public static bool Prefix(ItemBlueprint bp, BasePlayer owner, ProtoBuf.Item.InstanceData instanceData, int amount, int skinID, Item fromTempBlueprint, bool free, ref ItemCrafter __instance, out bool __result)
		{
			if (!__instance.CanCraft(bp, amount, free))
			{
				__result = false;
				return false;
			}
			++__instance.taskUID;

			var task = Facepunch.Pool.Get<ItemCraftTask>();
			task.blueprint = bp;

			if (!free)
				__instance.CollectIngredients(bp, task, amount, owner);
			task.endTime = 0.0f;
			task.taskUID = __instance.taskUID;
			task.owner = owner;
			task.instanceData = instanceData;
			if (task.instanceData != null)
				task.instanceData.ShouldPool = false;
			task.amount = amount;
			task.skinID = skinID;
			if (fromTempBlueprint != null && task.takenItems != null)
			{
				fromTempBlueprint.RemoveFromContainer();
				task.takenItems.Add(fromTempBlueprint);
				task.conditionScale = 0.5f;
			}

			var obj = HookCaller.CallStaticHook("OnItemCraft", task, owner, fromTempBlueprint);
			if (obj is bool)
			{
				if (fromTempBlueprint != null && task.instanceData != null)
					fromTempBlueprint.instanceData = task.instanceData;
				__result = (bool)obj;
				return false;
			}
			__instance.queue.AddLast(task);
			if (task.owner != null)
				task.owner.Command("note.craft_add", task.taskUID, task.blueprint.targetItem.itemid, amount, task.skinID);
			__result = true;
			return false;
		}
	}
}
