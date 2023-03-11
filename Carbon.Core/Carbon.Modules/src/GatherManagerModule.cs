using System;
using System.Collections.Generic;
using Carbon.Base;
using UnityEngine;
using static BaseEntity;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community
 * Copyright (c) 2023 kasvoton
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class GatherManagerModule : CarbonModule<GatherManagerConfig, EmptyModuleData>
{
	public override string Name => "GatherManager";
	public override Type Type => typeof(GatherManagerModule);

	#region Hooks

	private object OnCollectiblePickup(CollectibleEntity entity, BasePlayer reciever, bool eat)
	{
		foreach (var itemAmount in entity.itemList)
		{
			var item = ByDefinition(itemAmount.itemDef, (int)itemAmount.amount, 0, 0);
			if (item == null)
			{
				continue;
			}

			if (eat && item.info.category == ItemCategory.Food && reciever != null)
			{
				var component = item.info.GetComponent<ItemModConsume>();
				if (component != null)
				{
					component.DoAction(item, reciever);
					continue;
				}
			}

			if ((bool)reciever)
			{
				reciever.GiveItem(item, GiveItemReason.ResourceHarvested);
			}
			else
			{
				item.Drop(entity.transform.position + Vector3.up * 0.5f, Vector3.up);
			}
		}

		entity.itemList = null;
		if (entity.pickupEffect.isValid)
		{
			Effect.server.Run(entity.pickupEffect.resourcePath, entity.transform.position, entity.transform.up);
		}

		var randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(entity.prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(reciever, entity.transform.position);
		}

		entity.Kill();
		return false;
	}
	private void OnExcavatorGather(ExcavatorArm arm, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, 3);
	}
	private void OnQuarryGather(MiningQuarry quarry, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, 3);
	}
	private void OnGrowableGathered(GrowableEntity entity, Item item, BasePlayer player)
	{
		item.amount = GetAmount(item.info, item.amount, 1);
	}
	private void OnItemResearch(ResearchTable table, Item targetItem, BasePlayer player)
	{
		table.researchDuration = Config.ResearchDuration;
	}
	private void OnDispenserBonus(ResourceDispenser dispenser, BasePlayer player, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, 1);
	}

	private object ICraftDurationMultiplier()
	{
		return Config.CraftingSpeedMultiplier;
	}
	private object IRecyclerThinkSpeed()
	{
		return Config.RecycleTick;
	}
	private object IVendingBuyDuration()
	{
		return Config.VendingMachineBuyDuration;
	}

	private object IMixingSpeedMultiplier(MixingTable table, float originalValue)
	{
		if (table.currentRecipe == null) return null;

		if (originalValue == table.currentRecipe.MixingDuration * table.currentQuantity)
		{
			return Config.MixingSpeedMultiplier;
		}

		return null;
	}

	#endregion

	#region Helpers

	internal Item ByID(int itemID, int amount, ulong skin, int kind)
	{
		return ByDefinition(ItemManager.FindItemDefinition(itemID), amount, skin, kind);
	}
	internal Item ByDefinition(ItemDefinition itemDefinition, int amount, ulong skin, int kind)
	{
		return ItemManager.Create(itemDefinition, GetAmount(itemDefinition, amount, kind), skin);
	}
	internal int GetAmount(ItemDefinition itemDefinition, int amount, int kind)
	{
		var dictionary = kind switch
		{
			0 => Config.Pickup,
			1 => Config.Gather,
			2 => Config.Quarry,
			3 => Config.Excavator,
			_ => throw new Exception("Invalid CreateItemEx kind"),
		};

		if (!dictionary.TryGetValue(itemDefinition.shortname, out var multiply) && !dictionary.TryGetValue("*", out multiply))
		{
			multiply = 1f;
		}

		return Mathf.CeilToInt(amount * multiply);
	}

	#endregion
}

public class GatherManagerConfig
{
	public float RecycleTick = 5f;
	public float ResearchDuration = 10f;
	public float VendingMachineBuyDuration = 2.5f;
	public float CraftingSpeedMultiplier = 1f;
	public float MixingSpeedMultiplier = 1f;

	public Dictionary<string, float> Quarry = new()
	{
		["*"] = 1f
	};
	public Dictionary<string, float> Excavator = new()
	{
		["*"] = 1f
	};
	public Dictionary<string, float> Pickup = new()
	{
		["*"] = 1f,
		["seed.black.berry"] = 1f,
		["seed.blue.berry"] = 1f,
		["seed.corn"] = 1f,
		["seed.green.berry"] = 1f,
		["seed.hemp"] = 1f,
		["seed.potato"] = 1f,
		["seed.pumpkin"] = 1f,
		["seed.red.berry"] = 1f,
		["seed.white.berry"] = 1f,
		["seed.yellow.berry"] = 1f
	};
	public Dictionary<string, float> Gather = new()
	{
		["*"] = 1f,
		["skull.wolf"] = 1f,
		["skull.human"] = 1f
	};
}
