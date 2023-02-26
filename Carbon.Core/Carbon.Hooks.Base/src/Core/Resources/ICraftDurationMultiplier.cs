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
	public partial class Fixes_ItemCrafter
	{
		[HookAttribute.Patch("ICraftDurationMultiplier", typeof(ItemCrafter), "GetScaledDuration", new System.Type[] { typeof(ItemBlueprint), typeof(float) })]
		[HookAttribute.Identifier("78c74c0fd6624642815e7b147fda0802")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]

		public class Fixes_ItemCrafter_GetScaledDuration_78c74c0fd6624642815e7b147fda0802
		{
			public static void Postfix(ItemBlueprint bp, float workbenchLevel, ref float __result)
			{
				var hook = HookCaller.CallStaticHook("ICraftDurationMultiplier", bp, workbenchLevel);
				if (hook != null)
				{
					__result *= (float)hook;
				}
			}
		}
	}
}
