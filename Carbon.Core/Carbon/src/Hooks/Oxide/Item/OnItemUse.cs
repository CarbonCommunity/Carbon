///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnItemUse", typeof(int)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("this", typeof(Item))]
	[OxideHook.Parameter("amountToConsume", typeof(int))]
	[OxideHook.Info("Called when an item is used.")]
	[OxideHook.Info("Returning an int overrides the amount consumed.")]
	[OxideHook.Patch(typeof(Item), "UseItem")]
	public class Item_UseItem
	{
		public static void Prefix(ref int amountToConsume, ref Item __instance)
		{
			if (amountToConsume <= 0)
				return;

			var obj = HookCaller.CallStaticHook("OnItemUse", __instance, amountToConsume);

			if (obj is int)
			{
				amountToConsume = (int)obj;
			}
		}
	}
}
