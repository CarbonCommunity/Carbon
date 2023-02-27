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
	public partial class Fixes_VendingMachine
	{
		[HookAttribute.Patch("IVendingBuyDuration", typeof(VendingMachine), "GetBuyDuration", new System.Type[] {  })]
		[HookAttribute.Identifier("846de3cd762846e68206ffae9c97911f")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_VendingMachine_GetBuyDuration_846de3cd762846e68206ffae9c97911f
		{
			public static bool Prefix(VendingMachine __instance, ref float __result)
			{
				var hook = HookCaller.CallStaticHook("IVendingBuyDuration", __instance);

				if (hook is float value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}

		[HookAttribute.Patch("IVendingBuyDuration", typeof(InvisibleVendingMachine), "GetBuyDuration", new System.Type[] { })]
		[HookAttribute.Identifier("ac498841f2714f8f911d61d7713c142c")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_InvisibleVendingMachine_GetBuyDuration_ac498841f2714f8f911d61d7713c142c
		{
			public static bool Prefix(InvisibleVendingMachine __instance, ref float __result)
			{
				var hook = HookCaller.CallStaticHook("IVendingBuyDuration", __instance);

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
