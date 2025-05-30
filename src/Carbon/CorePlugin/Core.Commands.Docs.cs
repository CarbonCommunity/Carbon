using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("rcondocs.inv")]
	[AuthLevel(2)]
	private void RconDocs_Inv(ConsoleSystem.Arg arg)
	{
		var player = BasePlayer.Find(arg.GetString(0));

		if (player == null)
		{
			return;
		}

		using var main = Pool.Get<PooledList<Item>>();
		using var belt = Pool.Get<PooledList<Item>>();
		using var wear = Pool.Get<PooledList<Item>>();
		main.AddRange(player.inventory.containerMain.itemList);
		belt.AddRange(player.inventory.containerBelt.itemList);
		wear.AddRange(player.inventory.containerWear.itemList);

		arg.ReplyWithObject(new
		{
			Main = main.Select(x => new
			{
				ItemId = x.info?.itemid,
				ShortName = x.info?.shortname,
				Position = x.position,
				Amount = x.amount,
				MaxCondition = x.maxCondition,
				Condition = x.condition,
				ConditionNormalized = x.conditionNormalized,
				HasCondition = x.hasCondition,
			}),
			Belt = belt.Select(x => new
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
			Wear = wear.Select(x => new
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

	[ConsoleCommand("rcondocs.move")]
	[AuthLevel(2)]
	private void RconDocs_Move(ConsoleSystem.Arg arg)
	{
		var player = arg.GetPlayer(0);
		var fromContainer = GetContainer(arg.GetInt(1), player);
		var fromPosition = arg.GetInt(2);
		var fromItem = fromContainer.itemList.FirstOrDefault(x => x.position.Equals(fromPosition));

		var toContainerId = arg.GetInt(3);
		switch (toContainerId)
		{
			case 3: // Drop
				fromItem.Drop(player.GetDropPosition(), player.GetDropVelocity());
				break;
			case 4: // Discard
				fromItem.Remove();
				break;
			default:
				fromItem.MoveToContainer(GetContainer(toContainerId, player), arg.GetInt(4));
				break;
		}
	}

	private static ItemContainer GetContainer(int id, BasePlayer player)
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
