using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_ItemContainer
	{
		[HookAttribute.Patch("IDisallowSkinnedItemsFromBeingCraftable", "IDisallowSkinnedItemsFromBeingCraftable", typeof(ItemContainer), "GetAmount", new System.Type[] { typeof(int), typeof(bool) })]
		[HookAttribute.Identifier("14c9a1716b4248d4b707fbced49641fd")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_ItemContainer_14c9a1716b4248d4b707fbced49641fd : Patch
		{
			public static bool Prefix(int itemid, bool onlyUsableAmounts, out int __result, ref ItemContainer __instance)
			{
				if (HookCaller.CallStaticHook(2571830300) == null)
				{
					__result = default;
					return true;
				}

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
	}
}
