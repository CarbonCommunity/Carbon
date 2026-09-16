namespace Carbon.Extensions;

public static class ItemContainerEx
{
	public static int TakeSkinned(this ItemContainer container, int itemid, ulong skinId)
	{
		var num = 0;

		for(int i = 0; i < container.itemList.Count; i++)
		{
			var item = container.itemList[i];
			if (item.info.itemid == itemid && item.skin == skinId)
			{
				num += item.amount;
			}
		}

		return num;
	}
}
