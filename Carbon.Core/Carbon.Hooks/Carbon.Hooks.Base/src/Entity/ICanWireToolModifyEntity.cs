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
	public partial class Entity_WireTool
	{
		[HookAttribute.Patch("ICanWireToolModifyEntity", "ICanWireToolModifyEntity", typeof(WireTool), "CanModifyEntity", new System.Type[] { typeof(BasePlayer), typeof(BaseEntity) })]
		[HookAttribute.Identifier("bcfb498d9d9d42a1b376c820b5815d5d")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Entity_WireTool_CanModifyEntity_bcfb498d9d9d42a1b376c820b5815d5d : API.Hooks.Patch
		{
			public static bool Prefix(BasePlayer player, BaseEntity ent, ref bool __result)
			{
				var hook = HookCaller.CallStaticHook("ICanWireToolModifyEntity", player, ent);

				if(hook is bool result)
				{
					__result = result;
					return false;
				}

				return true;
			}
		}
	}
}
