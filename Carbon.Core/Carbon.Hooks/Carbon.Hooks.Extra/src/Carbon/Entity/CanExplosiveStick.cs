using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class Entity_TimedExplosive
	{
		[HookAttribute.Patch("CanExplosiveStick", "CanExplosiveStick", typeof(TimedExplosive), "CanStickTo", new System.Type[] { typeof(BaseEntity) })]
		[HookAttribute.Identifier("0e3eec7a5ccd43b68903f42d202b045d")]

		public class Entity_TimedExplosive_0e3eec7a5ccd43b68903f42d202b045d : Patch
		{
			public static bool Prefix(BaseEntity entity, ref TimedExplosive __instance, ref bool __result)
			{
				if (HookCaller.CallStaticHook("CanExplosiveStick", __instance, entity) is bool hookValue)
				{
					__result = hookValue;
					return false;
				}

				return true;
			}
		}
	}
}
