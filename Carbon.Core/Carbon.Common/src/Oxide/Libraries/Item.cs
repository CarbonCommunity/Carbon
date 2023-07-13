/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust.Libraries;

public class Item : Library
{
	public static global::Item GetItem(int itemId)
	{
		return ItemManager.CreateByItemID(itemId, 1, 0UL);
	}
}
