///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnEntityTakeDamage", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(ResourceEntity))]
	[OxideHook.Parameter("info", typeof(HitInfo))]
	[OxideHook.Info("Alternatively, modify the HitInfo object to change the damage.")]
	[OxideHook.Info("Return a false value overrides default behavior.")]
	[OxideHook.Patch(typeof(ResourceEntity), "OnAttacked")]
	public class ResourceEntity_OnAttacked
	{
		public static bool Prefix(HitInfo info, ref ResourceEntity __instance)
		{
			var obj = HookCaller.CallStaticHook("OnEntityTakeDamage", __instance, info);

			if (obj is bool)
			{
				return (bool)obj;
			}

			return true;
		}
	}
}
