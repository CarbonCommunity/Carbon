using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class Entity_CH74
	{
		[HookAttribute.Patch("ICanCH47CreateMapMarker", "ICanCH47CreateMapMarker", typeof(CH47Helicopter), "CreateMapMarker", new System.Type[] { })]
		[HookAttribute.Identifier("27a4e396a87248b189e3b8fbbab6d584")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Entity_CH47_TargetTick_27a4e396a87248b189e3b8fbbab6d584 : API.Hooks.Patch
		{
			public static bool Prefix(CH47Helicopter __instance)
			{
				if (HookCaller.CallStaticHook("ICanCH47CreateMapMarker", __instance) != null)
				{
					return false;
				}

				return true;
			}
		}
	}
}
