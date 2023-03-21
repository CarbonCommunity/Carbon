using System;
using System.Collections.Generic;
using API.Hooks;
using Network.Visibility;

// Copyright (c) 2022-2023 Carbon Community
// All rights reserved

namespace Carbon.Modules;

public partial class OptimisationsModule
{
	[HookAttribute.Patch("ae0577348a5140ea9aa861cd71c31e7c", "CircularNetworkDistance [Patch]", typeof(NetworkVisibilityGrid), "GetVisibleFrom")]
	[HookAttribute.Identifier("ae0577348a5140ea9aa861cd71c31e7c")]
	[HookAttribute.Options(HookFlags.Hidden)]
	public class NetworkVisibilityGrid_GetVisibleFrom_ae0577348a5140ea9aa861cd71c31e7c : API.Hooks.Patch
	{
		public static bool Prefix(NetworkVisibilityGrid __instance, Group group, List<Group> groups, int radius)
			=> GetVisibleFromCircle(__instance, group, groups, radius);
	}
}
