///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("CanCombineDroppedItem", typeof(object)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("this", typeof(DroppedItem))]
	[OxideHook.Parameter("droppedOn", typeof(DroppedItem))]
	[OxideHook.Info("Called when an item is dropped on another item.")]
	[OxideHook.Patch(typeof(DroppedItem), "OnDroppedOn")]
	public class DroppedItem_OnDroppedOn
	{
		public static bool Prefix(DroppedItem di, ref DroppedItem __instance)
		{
			if (__instance.item == null || di.item == null)
			{
				return false;
			}

			return Interface.CallHook("CanCombineDroppedItem", __instance, di) == null;
		}
	}
}
