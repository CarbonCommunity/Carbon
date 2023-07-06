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
	public partial class Fixes_VendingMachine
	{
		[HookAttribute.Patch("IVendingBuyDuration", "IVendingBuyDuration", typeof(VendingMachine), "GetBuyDuration", new System.Type[] { })]
		[HookAttribute.Identifier("846de3cd762846e68206ffae9c97911f")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_VendingMachine_GetBuyDuration_846de3cd762846e68206ffae9c97911f : Patch
		{
			public static bool Prefix(VendingMachine __instance, ref float __result)
			{
				var hook = HookCaller.CallStaticHook(2959446098, __instance);

				if (hook is float value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}

		[HookAttribute.Patch("IVendingBuyDuration", "IVendingBuyDuration", typeof(InvisibleVendingMachine), "GetBuyDuration", new System.Type[] { })]
		[HookAttribute.Identifier("ac498841f2714f8f911d61d7713c142c")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_InvisibleVendingMachine_GetBuyDuration_ac498841f2714f8f911d61d7713c142c : Patch
		{
			public static bool Prefix(InvisibleVendingMachine __instance, ref float __result)
			{
				var hook = HookCaller.CallStaticHook(2959446098, __instance);

				if (hook is float value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}
	}
}
