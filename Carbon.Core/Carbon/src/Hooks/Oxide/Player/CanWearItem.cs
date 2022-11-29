
using Oxide.Core;
using ProtoBuf;

///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Hooks
{
	[OxideHook("CanWearItem", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("inventory", typeof(PlayerInventory))]
	[OxideHook.Parameter("item", typeof(Item))]
	[OxideHook.Parameter("targetSlot", typeof(int))]
	[OxideHook.Info("Called when the player attempts to equip an item.")]
	[OxideHook.Patch(typeof(PlayerInventory), "CanWearItem")]
	public class PlayerInventory_CanWearItem
	{
		public static bool Prefix(Item item, int targetSlot, ref PlayerInventory __instance, out bool __result)
		{
			var component = item.info.GetComponent<ItemModWearable>();
			if (component == null)
			{
				__result = false;
				return false;
			}

			var obj = Interface.CallHook("CanWearItem", __instance, item, targetSlot);
			if (obj is bool boolObj)
			{
				__result = boolObj;
				return (bool)obj;
			}

			__result = default;
			return true;
		}
	}
}
