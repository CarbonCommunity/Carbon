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

public class RustOverridesModule : CarbonModule<RustOverridesConfig, EmptyModuleData>
{
	public override string Name => "RustOverrides";
	public override Type Type => typeof(RustOverridesModule);
	public override bool EnabledByDefault => true;

	private object IDisallowSkinnedItemsFromBeingCraftable()
	{
		if (ConfigInstance.DisallowSkinnedItemsFromBeingCraftable) return true;

		return null;
	}
}

public class RustOverridesConfig
{
	[JsonProperty("Disallow skinned items from being craftable")]
	public bool DisallowSkinnedItemsFromBeingCraftable = true;
}
