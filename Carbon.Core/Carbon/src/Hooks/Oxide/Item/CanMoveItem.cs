///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanMoveItem", typeof(object)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("item", typeof(Item))]
	[OxideHook.Parameter("this", typeof(PlayerInventory))]
	[OxideHook.Parameter("targetContainer", typeof(uint))]
	[OxideHook.Parameter("targetSlot", typeof(int))]
	[OxideHook.Parameter("amount", typeof(int))]
	[OxideHook.Info("Called when moving an item from one inventory slot to another.")]
	[OxideHook.Patch(typeof(PlayerInventory), "MoveItem")]
	public class CanMoveItem
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref PlayerInventory __instance)
		{
			var oldPosition = msg.read.Position;
			var num = msg.read.UInt32();
			var num2 = msg.read.UInt32();
			var num3 = (int)msg.read.Int8();
			var num4 = (int)msg.read.UInt32();
			var item = __instance.FindItemUID(num);
			if (item == null)
			{
				msg.player.ChatMessage("Invalid item (" + num + ")");
				return false;
			}
			msg.read.Position = oldPosition;
			return HookExecutor.CallStaticHook("CanMoveItem", item, __instance, num2, num3, num4) == null;
		}
	}
}
