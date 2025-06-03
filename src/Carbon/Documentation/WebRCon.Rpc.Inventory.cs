namespace Carbon.Documentation;

public static partial class WebRCon
{
	[DocsRpc]
	private static void SendPlayerInventory(ConsoleSystem.Arg arg)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(arg.GetULong(1));

		if (player == null)
		{
			return;
		}

		arg.ReplyWithObject(new
		{
			ActiveSlot = player.GetActiveItem()?.position ?? -1,
			Main = player.inventory.containerMain.itemList.Select(x => new
			{
				ItemId = x.info?.itemid,
				ShortName = x.info?.shortname,
				Position = x.position,
				Amount = x.amount,
				MaxCondition = x.maxCondition,
				Condition = x.condition,
				ConditionNormalized = x.conditionNormalized,
				HasCondition = x.hasCondition
			}),
			Belt = player.inventory.containerBelt.itemList.Select(x => new
			{
				ItemId = x.info?.itemid,
				ShortName = x.info?.shortname,
				Position = x.position,
				Amount = x.amount,
				MaxCondition = x.maxCondition,
				Condition = x.condition,
				ConditionNormalized = x.conditionNormalized,
				HasCondition = x.hasCondition
			}),
			Wear = player.inventory.containerWear.itemList.Select(x => new
			{
				ItemId = x.info?.itemid,
				ShortName = x.info?.shortname,
				Position = x.position,
				Amount = x.amount,
				MaxCondition = x.maxCondition,
				Condition = x.condition,
				ConditionNormalized = x.conditionNormalized,
				HasCondition = x.hasCondition
			})
		});
	}

	[DocsRpc]
	private static void MoveInventoryItem(ConsoleSystem.Arg arg)
	{
		var player = BasePlayer.FindAwakeOrSleepingByID(arg.GetULong(1));

		if (player == null)
		{
			return;
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
	}

	private static ItemContainer FindContainer(int id, BasePlayer player)
	{
		switch (id)
		{
			case 0:
				return player.inventory.containerMain;
			case 1:
				return player.inventory.containerBelt;
			case 2:
				return player.inventory.containerWear;
		}

		return null;
	}
}
