using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_MixingTable
	{
		[HookAttribute.Patch("IMixingSpeedMultiplier", "IMixingSpeedMultiplier", typeof(MixingTable), "set_RemainingMixTime", new System.Type[] { typeof(float) })]
		[HookAttribute.Identifier("0ad5ad2cc4224102be7e9ab434815462")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_MixingTable_set_RemainingMixTime_0ad5ad2cc4224102be7e9ab434815462 : Patch
		{
			public static void Prefix(ref float value, ref MixingTable __instance)
			{
				var hook = HookCaller.CallStaticHook(2901256393, __instance, value);

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
