using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fixes
{
	public partial class Fixes_MixingTable
	{
		[HookAttribute.Patch("IMixingSpeedMultiplier", typeof(MixingTable), "set_RemainingMixTime", new System.Type[] { typeof(float) })]
		[HookAttribute.Identifier("0ad5ad2cc4224102be7e9ab434815462")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_MixingTable_set_RemainingMixTime_0ad5ad2cc4224102be7e9ab434815462 : API.Hooks.Patch
		{
			public static void Prefix(ref float value, ref MixingTable __instance)
			{
				var hook = HookCaller.CallStaticHook("IMixingSpeedMultiplier", __instance, value);

				if (hook is float overridenValue)
				{
					if (value <= 0f)
					{
						value = 0f;
					}
					else
					{
						value /= overridenValue;
					}
				}
			}
		}
	}
}
