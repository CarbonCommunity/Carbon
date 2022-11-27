using Carbon.Base;
using Carbon.Modules;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fixes
{
	public partial class Fixes_ItemContainer
	{
		/*
		[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
		[CarbonHook("IItemContainerAmountPatch"), CarbonHook.Category(Hook.Category.Enum.Core)]
		[CarbonHook.Patch(typeof(ItemContainer), "GetAmount")]
		*/

		public class Fixes_ItemContainer_GetAmount_14c9a1716b4248d4b707fbced49641fd
		{
			public static Metadata metadata = new Metadata("IItemContainerAmountPatch",
				typeof(ItemContainer), "GetAmount", new System.Type[] { typeof(int), typeof(bool) });

			static Fixes_ItemContainer_GetAmount_14c9a1716b4248d4b707fbced49641fd()
			{
				metadata.SetAlwaysPatch(true);
				metadata.SetHidden(true);
			}

			public static bool Prefix(int itemid, bool onlyUsableAmounts, out int __result, ref ItemContainer __instance)
			{
				var overrides = BaseModule.GetModule<RustOverridesModule>();
				if (!overrides.ConfigInstance.Enabled || !overrides.Config.DisallowSkinnedItemsFromBeingCraftable)
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