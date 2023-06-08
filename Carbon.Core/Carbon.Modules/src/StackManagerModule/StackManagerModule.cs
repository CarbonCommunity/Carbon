using System;
using System.Collections.Generic;
using Carbon.Base;
using Carbon.Extensions;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 kasvoton
 * All rights reserved.
 *
 */

namespace Carbon.Modules;
#pragma warning disable IDE0051

public class StackManagerModule : CarbonModule<StackManagerConfig, StackManagerData>
{
	public override string Name => "StackManager";
	public override bool ForceModded => true;
	public override Type Type => typeof(StackManagerModule);

	public override bool EnabledByDefault => false;

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (!initialized || ItemManager.itemList == null) return;

		foreach (var category in ConfigInstance.Categories)
		{
			foreach (var item in ItemManager.itemList)
			{
				if (IsBypassed(item) || item.category != category.Key || ConfigInstance.Blacklist.Contains(item.shortname) || ConfigInstance.Items.ContainsKey(item.shortname)) continue;

				DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

				if (originalStack > 0) item.stackable = Mathf.Clamp((int)(originalStack * category.Value * ConfigInstance.GlobalMultiplier), 1, int.MaxValue);
			}
		}

		foreach (var item in ItemManager.itemList)
		{
			if (IsBypassed(item) || !ConfigInstance.Items.ContainsKey(item.shortname)) continue;

			var value = ConfigInstance.Items[item.shortname];

			DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

			if (originalStack > 0) item.stackable = Mathf.Clamp((int)(value * ConfigInstance.GlobalItemsMultiplier), 1, int.MaxValue);
		}
	}
	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		if (!initialized) return;

		Logger.Log("Rolling back item manager");

		foreach (var category in ConfigInstance.Categories)
		{
			foreach (var item in ItemManager.itemList)
			{
				if (item.category != category.Key || ConfigInstance.Blacklist.Contains(item.shortname) || ConfigInstance.Items.ContainsKey(item.shortname)) continue;

				DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

				if (originalStack > 0) item.stackable = originalStack.Clamp(1, int.MaxValue);
			}
		}

		foreach (var item in ItemManager.itemList)
		{
			if (!ConfigInstance.Items.ContainsKey(item.shortname)) continue;

			DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

			if (originalStack > 0) item.stackable = originalStack.Clamp(1, int.MaxValue);
		}
	}

	public bool IsBypassed(ItemDefinition definition)
	{
		if (definition.itemMods == null || definition.itemMods.Length == 0) return false;

		foreach (var mod in definition.itemMods)
		{
			if ((ConfigInstance.ProhibitItemContainerStacking && mod is ItemModContainer) ||
				(ConfigInstance.ProhibitItemConsumableContainerStacking && mod is ItemModConsumeContents) ||
				(ConfigInstance.ProhibitItemFishableStacking && mod is ItemModFishable) ||
				mod is ItemModPhoto)
			{
				return true;
			}
		}

		return false;
	}

	public override bool PreLoadShouldSave(bool newConfig, bool newData)
	{
		if (newConfig)
		{
			ConfigInstance.Blacklist.Add("water");
			ConfigInstance.Blacklist.Add("water.salt");
			return true;
		}

		return false;
	}
	public override void Load()
	{
		base.Load();

		OnEnableStatus();
	}

	public override void OnServerInit()
	{
		var hasChanged = false;
		foreach (var item in ItemManager.itemList)
		{
			if (!DataInstance.Items.ContainsKey(item.shortname))
			{
				DataInstance.Items.Add(item.shortname, item.stackable);

				hasChanged = true;
			}
		}

		if (hasChanged) Save();

		base.OnServerInit();
	}
}

public class StackManagerConfig
{
	public float GlobalMultiplier = 1f;
	public float GlobalItemsMultiplier = 1f;
	public bool ProhibitItemContainerStacking = false;
	public bool ProhibitItemConsumableContainerStacking = true;
	public bool ProhibitItemFishableStacking = true;

	public Dictionary<ItemCategory, float> Categories = new()
	{
		{ ItemCategory.Ammunition, 1 },
		{ ItemCategory.Attire, 1 },
		{ ItemCategory.Component, 1 },
		{ ItemCategory.Construction, 1 },
		{ ItemCategory.Electrical, 1 },
		{ ItemCategory.Food, 1 },
		{ ItemCategory.Fun, 1 },
		{ ItemCategory.Items, 1 },
		{ ItemCategory.Medical, 1 },
		{ ItemCategory.Misc, 1 },
		{ ItemCategory.Resources, 1 },
		{ ItemCategory.Tool, 1 },
		{ ItemCategory.Traps, 1 },
		{ ItemCategory.Weapon, 1 }
	};

	public Dictionary<string, float> Items = new();
	public HashSet<string> Blacklist = new();
}
public class StackManagerData
{
	public Dictionary<string, int> Items = new();
}
