///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Harmony;

[HarmonyPatch(typeof(ItemContainer), "GetAmount")]
public class ItemContainer_GetAmount
{
	public static bool Prefix (int itemid, bool onlyUsableAmounts, out int __result, ref ItemContainer __instance)
	{
		var num = 0;
		foreach (var item in __instance.itemList)
		{
			if (item.info.itemid == itemid && item.skin == 0 && (!onlyUsableAmounts || !item.IsBusy()))
			{
				num += item.amount;
			}
		}

		__result = num;
		return false;
	}
}
