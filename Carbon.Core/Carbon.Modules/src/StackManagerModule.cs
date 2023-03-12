using System;
using System.Collections.Generic;
using Carbon.Base;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 kasvoton
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class StackManagerModule : CarbonModule<StackManagerConfig, StackManagerData>
{
	public override string Name => "StackManager";
	public override bool ForceModded => true;
	public override Type Type => typeof(StackManagerModule);

	private void OnServerInitialized()
	{
		var hasChanged = false;
		foreach (var item in ItemManager.itemList)
		{
			if (!DataInstance.Items.ContainsKey(item.shortname))
			{
				DataInstance.Items.Add(item.shortname, item.stackable);
			}

			hasChanged = true;
		}

		if (hasChanged) Save();
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (!initialized || ItemManager.itemList == null) return;

		foreach (var category in ConfigInstance.Categories)
		{
			foreach (var item in ItemManager.itemList)
			{
				if (item.category != category.Key || ConfigInstance.Blacklist.Contains(item.shortname) || ConfigInstance.Items.ContainsKey(item.shortname)) continue;

				DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

				item.stackable = Mathf.Clamp((int)(originalStack * category.Value * ConfigInstance.GlobalMultiplier), 1, int.MaxValue);
			}
		}

		foreach (var item in ItemManager.itemList)
		{
			if (!ConfigInstance.Items.ContainsKey(item.shortname)) continue;

			var multiplier = ConfigInstance.Items[item.shortname];

			DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

			item.stackable = Mathf.Clamp((int)(originalStack * multiplier * ConfigInstance.GlobalMultiplier), 1, int.MaxValue);
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

				item.stackable = originalStack;
			}
		}

		foreach (var item in ItemManager.itemList)
		{
			if (!ConfigInstance.Items.ContainsKey(item.shortname)) continue;

			DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

			item.stackable = originalStack;
		}
	}
}

public class StackManagerConfig
{
	public float GlobalMultiplier = 1f;

	public HashSet<string> Blacklist = new()
	{
		"water",
		"water.salt"
	};

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

	public Dictionary<string, float> Items = new()
	{
		{ "explosive.timed", 1 }
	};
}
public class StackManagerData
{
	public Dictionary<string, int> Items = new();
}
