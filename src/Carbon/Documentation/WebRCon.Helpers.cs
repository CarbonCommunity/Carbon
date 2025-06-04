namespace Carbon.Documentation;

public static partial class WebRCon
{
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

	private static object ParseEntity(BaseEntity entity)
	{
		return new
		{
			NetId = entity.net.ID.Value,
			Name = entity.name,
			Flags = entity.flags
		};
	}

	private static object ParseItem(Item item)
	{
		return new
		{
			ItemId = item.info?.itemid,
			ShortName = item.info?.shortname,
			Position = item.position,
			Amount = item.amount,
			MaxCondition = item.maxCondition,
			Condition = item.condition,
			ConditionNormalized = item.conditionNormalized,
			HasCondition = item.hasCondition
		};
	}
}
