using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class RustEditModule
{
	[HookAttribute.Patch("ICanCH47CreateMapMarker", "ICanCH47CreateMapMarker", typeof(CH47Helicopter), "CreateMapMarker", new System.Type[] { })]
	[HookAttribute.Identifier("27a4e396a87248b189e3b8fbbab6d584")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Entity_CH47_TargetTick_27a4e396a87248b189e3b8fbbab6d584 : API.Hooks.Patch
	{
		public static bool Prefix(CH47Helicopter __instance)
		{
			if (HookCaller.CallStaticHook(1257940220, __instance) != null)
			{
				return false;
			}

			return true;
		}
	}
}
