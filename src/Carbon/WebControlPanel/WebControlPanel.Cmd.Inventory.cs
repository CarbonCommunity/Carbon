namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	private static Response CMD_SendPlayerInventory(ConsoleSystem.Arg arg)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(arg.GetULong(1));

		if (player == null)
		{
			return default;
		}

		var lootedEntity = player.inventory.loot.entitySource as StorageContainer;

		return GetResponse(new
		{
			ActiveSlot = player.GetActiveItem()?.position ?? -1,
			Main = player.inventory.containerMain.itemList.Select(x => ParseItem(x)),
			Belt = player.inventory.containerBelt.itemList.Select(x => ParseItem(x)),
			Wear = player.inventory.containerWear.itemList.Select(x => ParseItem(x)),
			Loot = lootedEntity == null ? null : new
			{
				Panel = lootedEntity.panelName,
				Items = lootedEntity.inventory.itemList.Select(x => ParseItem(x))
			}
		});
	}

	[WebCall]
	private static Response CMD_MoveInventoryItem(ConsoleSystem.Arg arg)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(arg.GetULong(1));

		if (player == null)
		{
			return default;
		}

		var fromContainer = FindContainer(arg.GetInt(2), player);
		var fromPosition = arg.GetInt(3);
		var fromItem = fromContainer.itemList.FirstOrDefault(x => x.position.Equals(fromPosition));

		var toContainerId = arg.GetInt(4);
		switch (toContainerId)
		{
			case 10: // Drop
				fromItem.Drop(player.GetDropPosition(), player.GetDropVelocity());
				break;
			case 11: // Discard
				fromItem.Remove();
				break;
			default:
				fromItem.MoveToContainer(FindContainer(toContainerId, player), arg.GetInt(5));
				break;
		}

		return GetResponse(arg);
	}
}
