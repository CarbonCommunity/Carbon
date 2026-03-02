namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PlayersInventory)]
	private static void RPC_SendPlayerInventory(BridgeRead read)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(read.UInt64());
		if (!player.IsValid())
		{
			return;
		}

		var inventory = player.inventory;
		var lootedEntity = inventory.loot.entitySource as StorageContainer;
		var write = StartRpcResponse();
		write.WriteObject(player.GetActiveItem()?.position ?? -1);
		write.WriteObject(inventory.containerMain.itemList.Count);
		for (int i = 0; i < inventory.containerMain.itemList.Count; i++)
		{
			new ItemInfo(inventory.containerMain.itemList[i]).Serialize(write);
		}
		write.WriteObject(inventory.containerBelt.itemList.Count);
		for (int i = 0; i < inventory.containerBelt.itemList.Count; i++)
		{
			new ItemInfo(inventory.containerBelt.itemList[i]).Serialize(write);
		}
		write.WriteObject(inventory.containerWear.itemList.Count);
		for (int i = 0; i < inventory.containerWear.itemList.Count; i++)
		{
			new ItemInfo(inventory.containerWear.itemList[i]).Serialize(write);
		}
		write.WriteObject(lootedEntity != null);
		if (lootedEntity != null)
		{
			write.WriteObject(lootedEntity.panelName);
			write.WriteObject(lootedEntity.inventory.itemList.Count);
			for (int i = 0; i < lootedEntity.inventory.itemList.Count; i++)
			{
				new ItemInfo(lootedEntity.inventory.itemList[i]).Serialize(write);
			}
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PlayersInventory)]
	private static void RPC_MoveInventoryItem(BridgeRead read)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(read.UInt64());
		if (!player.IsValid())
		{
			return;
		}
		var fromContainer = FindContainer(read.Int32(), player);
		var fromPosition = read.Int32();
		var fromItem = fromContainer.itemList.FirstOrDefault(x => x.position.Equals(fromPosition));
		var toContainerId = read.Int32();
		switch (toContainerId)
		{
			case 10: // Drop
				fromItem.Drop(player.GetDropPosition(), player.GetDropVelocity());
				break;
			case 11: // Discard
				fromItem.Remove();
				break;
			default:
				fromItem.MoveToContainer(FindContainer(toContainerId, player), read.Int32());
				break;
		}
	}

	public struct ItemInfo(Item item)
	{
		private int itemId = item.info?.itemid ?? 0;
		private string shortName = item.info?.shortname ?? "";
		private int position = item.position;
		private int amount = item.amount;
		private float maxCondition = item.maxCondition;
		private float condition = item.condition;
		private float conditionNormalized = item.conditionNormalized;
		private bool hasCondition = item.hasCondition;

		public void Serialize(BridgeWrite write)
		{
			write.WriteObject(itemId);
			write.WriteObject(shortName);
			write.WriteObject(amount);
			write.WriteObject(position);
			write.WriteObject(maxCondition);
			write.WriteObject(condition);
			write.WriteObject(conditionNormalized);
			write.WriteObject(hasCondition);
		}
	}
}
