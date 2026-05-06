using System;
using System.Collections.Generic;
using Carbon.Base;
using Facepunch.Rust;
using Oxide.Core;
using UnityEngine;
using static BaseEntity;

/*
 *
 * Copyright (c) 2022 kasvoton, under the GNU v3 license rights
 * Copyright (c) 2022-2024 Carbon Community, under the GNU v3 license rights
 *
 */

namespace Carbon.Modules;
#pragma warning disable IDE0051

public partial class GatherManagerModule : CarbonModule<GatherManagerConfig, EmptyModuleData>
{
	public static GatherManagerModule Singleton { get; internal set; }

	public override string Name => "GatherManager";
	public override VersionNumber Version => new(1, 0, 0);
	public override bool ForceModded => true;
	public override Type Type => typeof(GatherManagerModule);

	public override bool EnabledByDefault => false;

	public enum KindTypes
	{
		Pickup = 0,
		Gather = 1,
		Quarry = 2,
		Excavator = 3
	}

	public override void Init()
	{
		base.Init();

		Singleton = this;
	}

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
				Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, entity, reciever);
				reciever.GiveItem(item, GiveItemReason.ResourceHarvested);
			}
			else
			{
				item.Drop(entity.transform.position + Vector3.up * 0.5f, Vector3.up);
			}
		}

		if (entity.pickupEffect.isValid)
		{
			Effect.server.Run(entity.pickupEffect.resourcePath, entity.transform.position, entity.transform.up);
		}

		var randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(entity.prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(reciever, entity.transform.position);
		}

		NextFrame(() =>
		{
			if (entity == null || entity.IsDestroyed) return;

			entity.Kill();
		});

		return false;
	}
	private void OnExcavatorGather(ExcavatorArm arm, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, KindTypes.Excavator);
	}
	private void OnQuarryGather(MiningQuarry quarry, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, KindTypes.Quarry);
	}
	private void OnGrowableGathered(GrowableEntity entity, Item item, BasePlayer player)
	{
		item.amount = GetAmount(item.info, item.amount, KindTypes.Gather);
	}
	private void OnDispenserBonus(ResourceDispenser dispenser, BasePlayer player, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, KindTypes.Gather);
	}
	private void OnDispenserGather(ResourceDispenser dispenser, BaseEntity entity, Item item)
	{
		item.amount = GetAmount(item.info, item.amount, KindTypes.Gather);
	}
	private void OnFishCatch(Item item)
	{
		item.amount = GetAmount(item.info, item.amount, KindTypes.Gather);
	}

	#endregion

	#region Helpers

	private Item ByID(int itemID, int amount, ulong skin, KindTypes kind)
	{
		return ByDefinition(ItemManager.FindItemDefinition(itemID), amount, skin, kind);
	}
	private Item ByDefinition(ItemDefinition itemDefinition, int amount, ulong skin, KindTypes kind)
	{
		return ItemManager.Create(itemDefinition, GetAmount(itemDefinition, amount, kind), skin);
	}
	private int GetAmount(ItemDefinition itemDefinition, int amount, KindTypes kind)
	{
		var dictionary = kind switch
		{
			KindTypes.Pickup => ConfigInstance.Pickup,
			KindTypes.Gather => ConfigInstance.Gather,
			KindTypes.Quarry => ConfigInstance.Quarry,
			KindTypes.Excavator => ConfigInstance.Excavator,
			_ => throw new Exception("Invalid GetAmount kind"),
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
