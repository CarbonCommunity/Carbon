///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using System;
using Carbon.Base;
using Newtonsoft.Json;

namespace Carbon.Modules
{
	public class RustOverridesModule : CarbonModule<RustOverridesConfig, RustOverridesData>
	{
		public override string Name => "RustOverrides";
		public override Type Type => typeof(RustOverridesModule);
		public override bool EnabledByDefault => true;
	}

	public class RustOverridesConfig
	{
		[JsonProperty("Disallow skinned items from being craftable")]
		public bool DisallowSkinnedItemsFromBeingCraftable = true;
	}
	public class RustOverridesData
	{

	}
}
