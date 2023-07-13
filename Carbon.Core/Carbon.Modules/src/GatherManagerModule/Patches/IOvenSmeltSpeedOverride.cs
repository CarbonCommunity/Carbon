using System;
using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class GatherManagerModule
{
	[HookAttribute.Patch("IOvenSmeltSpeedOverride", "IOvenSmeltSpeedOverride", typeof(BaseOven), "StartCooking", new System.Type[] { })]
	[HookAttribute.Identifier("91cf6d3873704b12b39fd555c376a652")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class BaseOven_StartCooking_91cf6d3873704b12b39fd555c376a652 : API.Hooks.Patch
	{
		public static bool Prefix(BaseOven __instance)
		{
			if (HookCaller.CallStaticHook(4131997659, __instance) is float hookResult)
			{
				if (__instance.FindBurnable() == null && !__instance.CanRunWithNoFuel)
				{
					return false;
				}
				__instance.inventory.temperature = __instance.cookingTemperature;
				__instance.UpdateAttachmentTemperature();
				__instance.InvokeRepeating(new Action(__instance.Cook), hookResult, hookResult);
				__instance.SetFlag(global::BaseEntity.Flags.On, true, false, true);
				return false;
			}

			return true;
		}
	}
}
