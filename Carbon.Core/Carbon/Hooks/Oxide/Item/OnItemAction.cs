///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnItemAction", typeof(object)), OxideHook.Category(Hook.Category.Enum.Item)]
	[OxideHook.Parameter("item", typeof(Item))]
	[OxideHook.Parameter("action", typeof(string))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a button is clicked on an item in the inventory (drop, unwrap, etc).")]
	[OxideHook.Patch(typeof(PlayerInventory), "ItemCmd")]
	public class PlayerInventory_ItemCmd
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref PlayerInventory __instance)
		{
			if (msg.player != null && msg.player.IsWounded())
			{
				return false;
			}

			var oldPosition = msg.read.Position;
			var id = msg.read.UInt32();
			var text = msg.read.String(256);
			var item = __instance.FindItemUID(id);
			msg.read.Position = oldPosition;

			if (item == null) return false;

			return Interface.CallHook("OnItemAction", item, text, msg.player) == null;
		}
	}
}
