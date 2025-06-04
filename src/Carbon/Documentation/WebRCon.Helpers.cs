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
}
