using System;
using Carbon.Base;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

#pragma warning disable IDE0051

public class RustOverridesModule : CarbonModule<RustOverridesConfig, EmptyModuleData>
{
	public override string Name => "RustOverrides";
	public override Type Type => typeof(RustOverridesModule);
	public override bool ForceModded => true;

	public override bool EnabledByDefault => false;

	private object IDisallowSkinnedItemsFromBeingCraftable()
	{
		if (ConfigInstance.DisallowSkinnedItemsFromBeingCraftable) return true;

		return null;
	}

	[HookPriority(Priorities.Highest)]
	private object OnServerMessage(string message, string name)
	{
		if (!ConfigInstance.NoGiveNotices) return null;

		return name == "SERVER" && message.Contains("gave");
	}
}

public class RustOverridesConfig
{
	[JsonProperty("Disallow skinned items from being craftable")]
	public bool DisallowSkinnedItemsFromBeingCraftable = false;

	[JsonProperty("No give notices")]
	public bool NoGiveNotices = false;
}
