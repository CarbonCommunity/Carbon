namespace Carbon.Documentation;

public class Structure
{

	public enum Flag
	{
		NoDropping = 1,
		NotStraightToBelt = 2,
		NotAllowedInBelt = 4,
		Backpack = 8,
	}

	public enum ItemCategory
	{
		Weapon,
		Construction,
		Items,
		Resources,
		Attire,
		Tool,
		Medical,
		Food,
		Ammunition,
		Traps,
		Misc,
		All,
		Common,
		Component,
		Search,
		Favourite,
		Electrical,
		Fun,
	}

	public enum Rarity
	{
		None,
		Common,
		Uncommon,
		Rare,
		VeryRare,
	}

	public enum spawnType
	{
		GENERIC,
		PLAYER,
		TOWN,
		AIRDROP,
		CRASHSITE,
		ROADSIDE,
	}

	public class Item
	{
		public long Id { get; set; }
		public string DisplayName { get; set; }
		public string ShortName { get; set; }
		public string Description { get; set; }
		public int Stack { get; set; }
		public bool Hidden { get; set; }
		public Flag Flags { get; set; }
		public ItemCategory Category { get; set; }
		public Rarity Rarity { get; set; }
	}

	public class Entity : Prefab
	{
	}

	public class Prefab
	{
		public string Type { get; set; }
		public string Path { get; set; }
		public string Name { get; set; }
		public string[] Components { get; set; }
		public uint ID { get; set; }
	}

	public class Blueprint
	{
		public Item Item { get; set; }
		public bool UserCraftable { get; set; }
		public Rarity Rarity { get; set; }
		public int CraftAmount { get; set; }
		public int ScrapRequired { get; set; }
		public int WorkbenchLevelRequired { get; set; }
		public bool NeedsSteamItem { get; set; }
		public bool NeedsSteamDLC { get; set; }
		public Ingredient[] Ingredients { get; set; }

		public class Ingredient
		{
			public Item Item { get; set; }
			public float Amount { get; set; }
		}
	}

	public class LootTable : Entity
	{
		public RangeItem[] Items { get; set; } = new RangeItem[0];
		public SpawnSlotItem[] SpawnSlotItems { get; set; } = new SpawnSlotItem[0];
		public int ScrapAmount { get; set; }
		public spawnType SpawnType { get; set; }

		public class RangeItem : Item
		{
			public float Amount { get; set; }
			public float MaxAmount { get; set; }
		}

		public class SpawnSlotItem
		{
			public RangeItem[] Items { get; set; } = new RangeItem[0];
			public int NumberToSpawn { get; set; }
			public float Probability { get; set; }
		}
	}

	public class Changelog
	{
		public string Date { get; set; }
		public string Version { get; set; }
		public Change[] Changes { get; set; }

		public class Change
		{
			public string Message { get; set; }
			public ChangeTypes Type { get; set; } = ChangeTypes.Added;
		}

		public enum ChangeTypes
		{
			Added = 0,
			Updated = 1,
			Removed = 2,
			Fixed = 3,
			Miscellaneous = 4
		}
	}

	public class HookOverride
	{
		public string[] Info { get; set; }
		public string ReturnType { get; set; }
		public Parameter[] Parameters { get; set; }

		public class Parameter
		{
			public string Type { get; set; }
			public string Name { get; set; }
		}
	}
}
