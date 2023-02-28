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

public class RustOverridesModule : CarbonModule<RustOverridesConfig, RustOverridesData>
{
	public override string Name => "RustOverrides";
	public override Type Type => typeof(RustOverridesModule);
	public override bool EnabledByDefault => true;

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		Subscribe("IDisallowSkinnedItemsFromBeingCraftable");
	}
	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);

		Unsubscribe("IDisallowSkinnedItemsFromBeingCraftable");
	}

	private object IDisallowSkinnedItemsFromBeingCraftable()
	{
		if (Config.DisallowSkinnedItemsFromBeingCraftable) return true;

		return null;
	}
}

public class RustOverridesConfig
{
	[JsonProperty("Disallow skinned items from being craftable")]
	public bool DisallowSkinnedItemsFromBeingCraftable = true;
}
public class RustOverridesData
{

}
