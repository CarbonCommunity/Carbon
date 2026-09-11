using Facepunch;

namespace Oxide.Core.Plugins;

public static class Extensions
{
	public static void Clear(this ItemContainer cont)
	{
		var items = Pool.Get<List<Item>>();
		items.AddRange(cont.itemList);

		foreach (var item in items)
		{
			item.Remove(.1f);
		}

		ItemManager.DoRemoves();

		Pool.FreeUnmanaged(ref items);
	}
}
