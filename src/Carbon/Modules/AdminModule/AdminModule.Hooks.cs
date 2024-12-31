using ExtensionMethods = Oxide.Core.ExtensionMethods;

namespace Carbon.Modules;

public partial class AdminModule
{
#if !MINIMAL
	[Conditional("!MINIMAL")]
	private void OnEntityDismounted(BaseMountable entity, BasePlayer player)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);
		if (!ap.GetStorage(tab, "wasviewingcam", false)) return;

		entity.Kill();
		Draw(player);

		Unsubscribe("OnEntityDismounted");
	}

	[Conditional("!MINIMAL")]
	private void OnPlayerLootEnd(PlayerLoot loot)
	{
		if (EntitiesTab.LastContainerLooter != null && loot.baseEntity == EntitiesTab.LastContainerLooter.Player)
		{
			Draw(EntitiesTab.LastContainerLooter.Player);
			EntitiesTab.LastContainerLooter = null;
			Unsubscribe("OnEntityVisibilityCheck");
			Unsubscribe("OnEntityDistanceCheck");
			Unsubscribe("CanAcceptItem");
		}
	}

	[Conditional("!MINIMAL")]
	private object OnEntityDistanceCheck(BaseEntity ent, BasePlayer player, uint id, string debugName, float maximumDistance)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);
		var lootedEnt = ap.GetStorage<BaseEntity>(tab, "lootedent");

		if (lootedEnt == null) return null;

		return true;
	}

	[Conditional("!MINIMAL")]
	private object OnEntityVisibilityCheck(BaseEntity ent, BasePlayer player, uint id, string debugName, float maximumDistance)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);
		var lootedEnt = ap.GetStorage<BaseEntity>(tab, "lootedent");

		if (lootedEnt == null) return null;

		return true;
	}

	[Conditional("!MINIMAL")]
	private void OnPlayerDisconnected(BasePlayer player)
	{
		Tab.OptionChart.Cache.ClearPlayerViewer(player.userID);

		if (PlayersTab.BlindedPlayers.Contains(player))
		{
			PlayersTab.BlindedPlayers.Remove(player);
		}

		StopSpectating(player);
	}

	[Conditional("!MINIMAL")]
	private object CanAcceptItem(ItemContainer container, Item item, int targetPos)
	{
		var owner = container.playerOwner;

		if (owner == null || container != owner.inventory.containerBelt || !ExtensionMethods.Contains(_backpacks, item.info.itemid))
		{
			return null;
		}

		if (!Singleton.HasAccess(owner, "entities.loot_players"))
		{
			return null;
		}
		OpenContainer(GetPlayerSession(owner), item.contents, null);
		return ItemContainer.CanAcceptResult.CannotAccept;
	}

	[Conditional("!MINIMAL")]
	private object IValidDismountPosition(BaseMountable mountable, BasePlayer player) => null;

	[Conditional("!MINIMAL")]
	private object IModBackpack(BaseMountable mountable, BasePlayer player) => null;

	public static bool AcceptOnBackpack(Item backpack)
	{
		return EntitiesTab.LastContainerLooter != null && EntitiesTab.LastContainerLooter.Player?.inventory?.loot?.containers[0] == backpack.contents;
	}
#endif
}
