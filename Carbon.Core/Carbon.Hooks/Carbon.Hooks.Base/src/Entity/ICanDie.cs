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
	public partial class Entity_BaseCombatEntity
	{
		[HookAttribute.Patch("ICanDie", "ICanDie", typeof(BaseCombatEntity), "Die", new System.Type[] { typeof(HitInfo) })]
		[HookAttribute.Identifier("6a89e1416f1b4327aba69593f3c43fb8")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Entity_BaseCombatEntity_TargetTick_6a89e1416f1b4327aba69593f3c43fb8 : API.Hooks.Patch
		{
			public static bool Prefix(HitInfo info, BaseCombatEntity __instance)
			{
				if (HookCaller.CallStaticHook("ICanDie", __instance, info) != null)
				{
					return false;
				}

				return true;
			}
		}
	}
}
