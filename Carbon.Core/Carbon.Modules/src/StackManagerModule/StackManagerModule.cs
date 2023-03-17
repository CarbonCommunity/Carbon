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
				if (item.category != category.Key || ConfigInstance.Blacklist.Contains(item.shortname) || ConfigInstance.Items.ContainsKey(item.shortname)) continue;

				DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

				if (originalStack > 0) item.stackable = Mathf.Clamp((int)(originalStack * category.Value * ConfigInstance.GlobalMultiplier), 1, int.MaxValue);
			}
		}

		foreach (var item in ItemManager.itemList)
		{
			if (!ConfigInstance.Items.ContainsKey(item.shortname)) continue;

			var value = ConfigInstance.Items[item.shortname];

			DataInstance.Items.TryGetValue(item.shortname, out var originalStack);

			if (originalStack > 0) item.stackable = Mathf.Clamp((int)(value * ConfigInstance.GlobalMultiplier), 1, int.MaxValue);
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
