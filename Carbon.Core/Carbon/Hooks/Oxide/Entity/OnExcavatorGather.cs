///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Hooks.Oxide.Entity
{
	[OxideHook("OnExcavatorGather", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("arm", typeof(ExcavatorArm))]
	[OxideHook.Parameter("item", typeof(Item))]
	[OxideHook.Info("Called right before moving gathered resource to container.")]
	[OxideHook.Patch(typeof(ExcavatorArm), "ProduceResources")]
	public class ExcavatorArm_ProduceResources
	{
		public static bool Prefix(ref ExcavatorArm __instance)
		{
			var num = __instance.resourceProductionTickRate / __instance.timeForFullResources;
			var num2 = __instance.resourcesToMine[__instance.resourceMiningIndex].amount * num;
			__instance.pendingResources[__instance.resourceMiningIndex].amount += num2;

			foreach (var itemAmount in __instance.pendingResources)
			{
				if (itemAmount.amount >= __instance.outputPiles.Count)
				{
					var num3 = Mathf.FloorToInt(itemAmount.amount / __instance.outputPiles.Count);
					itemAmount.amount -= num3 * 2;

					foreach (var excavatorOutputPile in __instance.outputPiles)
					{
						var item = ItemManager.Create(__instance.resourcesToMine[__instance.resourceMiningIndex].itemDef, num3, 0UL);
						if (Interface.CallHook("OnExcavatorGather", __instance, item) != null)
						{
							return false;
						}
						if (!item.MoveToContainer(excavatorOutputPile.inventory, -1, true, false, null))
						{
							item.Drop(excavatorOutputPile.GetDropPosition(), excavatorOutputPile.GetDropVelocity(), default);
						}
					}
				}
			}
			return false;
		}
	}
}
