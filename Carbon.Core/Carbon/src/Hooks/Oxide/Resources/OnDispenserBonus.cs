///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnDispenserBonus", typeof(Item)), OxideHook.Category(Hook.Category.Enum.Resources)]
	[OxideHook.Parameter("this", typeof(ResourceDispenser))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("item", typeof(Item))]
	[OxideHook.Info("Called before the player is given a bonus item for gathering.")]
	[OxideHook.Info("Returning an Item replaces the existing Item")]
	[OxideHook.Info("Returning a false value overrides default behavior")]
	[OxideHook.Patch(typeof(ResourceDispenser), "AssignFinishBonus")]
	public class ResourceDispenser_AssignFinishBonus
	{
		public static bool Prefix(BasePlayer player, float fraction, ref ResourceDispenser __instance)
		{
			__instance.SendMessage("FinishBonusAssigned", SendMessageOptions.DontRequireReceiver);
			if ((double)fraction <= 0.0 || __instance.finishBonus == null)
				return false;
			foreach (ItemAmount finishBonu in __instance.finishBonus)
			{
				int amountToGive = Mathf.CeilToInt((float)(int)finishBonu.amount * Mathf.Clamp01(fraction));
				int gatherBonus = __instance.CalculateGatherBonus((BaseEntity)player, finishBonu, (float)amountToGive);
				Item obj1 = ItemManager.Create(finishBonu.itemDef, amountToGive + gatherBonus);
				if (obj1 != null)
				{
					object obj2 = HookCaller.CallStaticHook("OnDispenserBonus", (object)__instance, (object)player, (object)obj1);
					if (obj2 is bool)
						if (!(bool)obj2)
							continue;
					if (obj2 is Item)
						obj1 = (Item)obj2;
					player.GiveItem(obj1, BaseEntity.GiveItemReason.ResourceHarvested);
				}
			}
			return false;
		}
	}
}
