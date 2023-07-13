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
	[HookAttribute.Patch("IPreTurretTargetTick", "IPreTurretTargetTick", typeof(AutoTurret), "TargetTick", new System.Type[] { })]
	[HookAttribute.Identifier("ccf1f97dbb4a49ac9320c5669afe2cbd")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Entity_AutoTurret_TargetTick_ccf1f97dbb4a49ac9320c5669afe2cbd : API.Hooks.Patch
	{
		public static bool Prefix(AutoTurret __instance)
		{
			if (HookCaller.CallStaticHook(3582323383, __instance) != null)
			{
				return false;
			}

			return true;
		}
	}
}
