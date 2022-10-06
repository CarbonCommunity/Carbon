///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Collections.Generic;
using System;
using Carbon.Core;
using UnityEngine;

namespace Carbon.Extended
{
	[OxideHook("OnItemCraftCancelled"), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("itemCraftTask", typeof(ItemCraftTask))]
	[OxideHook.Info("Called before an item has been crafted.")]
	[OxideHook.Patch(typeof(ItemCrafter), "CancelTask")]
	public class ItemCrafter_CancelTask
	{
		public static bool Prefix(int iID, bool ReturnItems, ref ItemCrafter __instance, out bool __result)
		{
			if (__instance.queue.Count == 0)
			{
				__result = false;
				return false;
			}
			ItemCraftTask itemCraftTask = System.Linq.Enumerable.FirstOrDefault<ItemCraftTask>((IEnumerable<ItemCraftTask>)__instance.queue, (Func<ItemCraftTask, bool>)(x => x.taskUID == iID && !x.cancelled));
			if (itemCraftTask == null)
			{
				__result = false;
				return false;
			}
			itemCraftTask.cancelled = true;
			if ((UnityEngine.Object)itemCraftTask.owner == (UnityEngine.Object)null)
			{
				__result = true;
				return false;
			}
			HookExecutor.CallStaticHook("OnItemCraftCancelled", (object)itemCraftTask);
			itemCraftTask.owner.Command("note.craft_done", (object)itemCraftTask.taskUID, (object)0);
			if (((itemCraftTask.takenItems == null ? 0 : (itemCraftTask.takenItems.Count > 0 ? 1 : 0)) & (ReturnItems ? 1 : 0)) != 0)
			{
				foreach (Item takenItem in itemCraftTask.takenItems)
				{
					if (takenItem != null && takenItem.amount > 0)
					{
						if (takenItem.IsBlueprint() && (UnityEngine.Object)takenItem.blueprintTargetDef == (UnityEngine.Object)itemCraftTask.blueprint.targetItem)
							takenItem.UseItem(itemCraftTask.numCrafted);
						if (takenItem.amount > 0 && !takenItem.MoveToContainer(itemCraftTask.owner.inventory.containerMain))
						{
							takenItem.Drop(itemCraftTask.owner.inventory.containerMain.dropPosition + UnityEngine.Random.value * Vector3.down + UnityEngine.Random.insideUnitSphere, itemCraftTask.owner.inventory.containerMain.dropVelocity);
							itemCraftTask.owner.Command("note.inv", (object)takenItem.info.itemid, (object)-takenItem.amount);
						}
					}
				}
			}
			__result = true;
			return false;
		}
	}
}
