using System.Collections.Generic;
using API.Hooks;
using Facepunch;

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
		[HookAttribute.Patch("IDisallowSkinnedItemsFromBeingCraftable", "IDisallowSkinnedItemsFromBeingCraftable", typeof(ItemContainer), "Take", new System.Type[] { typeof(List<Item>), typeof(int), typeof(int) })]
		[HookAttribute.Identifier("c44b4b824a274a5a96b9154a612d747a")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_ItemContainer_c44b4b824a274a5a96b9154a612d747a : Patch
		{
			public static bool Prefix(List<Item> collect, int itemid, int iAmount, out int __result, ref ItemContainer __instance)
			{
				if (HookCaller.CallStaticHook(2571830300) == null)
				{
					__result = default;
					return true;
				}

				var num = 0;
				if (iAmount == 0)
				{
					__result = num;
				}

				var list = Facepunch.Pool.GetList<Item>();
				foreach (var item in __instance.itemList)
				{
					if (item.info.itemid != itemid || item.skin != 0) continue;

					int num2 = iAmount - num;
					if (num2 > 0)
					{
						if (item.amount > num2)
						{
							item.MarkDirty();
							item.amount -= num2;
							num += num2;

							var item2 = ItemManager.CreateByItemID(itemid, 1, 0UL);
							item2.amount = num2;
							item2.CollectedForCrafting(__instance.playerOwner);

							if (collect != null)
							{
								collect.Add(item2);
								break;
							}
							break;
						}
						else
						{
							if (item.amount <= num2)
							{
								num += item.amount;
								list.Add(item);
								collect?.Add(item);
							}

							if (num == iAmount)
							{
								break;
							}
						}
					}
				}

				foreach (var item3 in list)
				{
					item3.RemoveFromContainer();
				}

				Facepunch.Pool.FreeList(ref list);
				__result = num;
				return false;
			}
		}
	}
}
