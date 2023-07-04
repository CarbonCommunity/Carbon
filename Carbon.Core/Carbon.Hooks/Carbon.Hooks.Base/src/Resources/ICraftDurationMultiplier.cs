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
	public partial class Fixes_ItemCrafter
	{
		[HookAttribute.Patch("ICraftDurationMultiplier", "ICraftDurationMultiplier", typeof(ItemCrafter), "GetScaledDuration", new System.Type[] { typeof(ItemBlueprint), typeof(float) })]
		[HookAttribute.Identifier("78c74c0fd6624642815e7b147fda0802")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_ItemCrafter_GetScaledDuration_78c74c0fd6624642815e7b147fda0802 : Patch
		{
			public static void Postfix(ItemBlueprint bp, float workbenchLevel, ref float __result)
			{
				var hook = HookCaller.CallStaticHook(4130008882, bp, workbenchLevel);

				if (hook is float value)
				{
					__result *= value;
				}
			}
		}
	}
}
