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
	[HookAttribute.Patch("ICanDie", "ICanDie", typeof(BaseCombatEntity), "Die", new System.Type[] { typeof(HitInfo) })]
	[HookAttribute.Identifier("6a89e1416f1b4327aba69593f3c43fb8")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Entity_BaseCombatEntity_Die_6a89e1416f1b4327aba69593f3c43fb8 : API.Hooks.Patch
	{
		public static bool Prefix(HitInfo info, BaseCombatEntity __instance)
		{
			if (HookCaller.CallStaticHook(3218212956, __instance, info) != null)
			{
				return false;
			}

			return true;
		}
	}
}
