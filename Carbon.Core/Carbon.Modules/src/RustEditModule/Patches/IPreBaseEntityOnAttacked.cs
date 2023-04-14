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
	[HookAttribute.Patch("IPreBaseEntityOnAttacked", "IPreBaseEntityOnAttacked", typeof(BaseEntity), "OnAttacked", new System.Type[] { typeof(HitInfo) })]
	[HookAttribute.Identifier("25cb3d11502d4da29aea9bfb5c53a661")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class BaseEntity_OnAttacked_25cb3d11502d4da29aea9bfb5c53a661 : API.Hooks.Patch
	{
		public static bool Prefix(BaseEntity __instance)
		{
			if (HookCaller.CallStaticHook("IPreBaseEntityOnAttacked", __instance) != null)
			{
				return false;
			}

			return true;
		}
	}
}
