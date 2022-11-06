///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnTurretTarget"), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(AutoTurret))]
	[OxideHook.Parameter("entity", typeof(BaseCombatEntity))]
	[OxideHook.Info("Called when an auto-turret attempts to target an entity.")]
	[OxideHook.Patch(typeof(AutoTurret), "SetTarget")]
	public class AutoTurret_SetTarget
	{
		public static bool Prefix(BaseCombatEntity targ, ref AutoTurret __instance)
		{
			return Interface.CallHook("OnTurretTarget", __instance, targ) == null;
		}
	}
}
