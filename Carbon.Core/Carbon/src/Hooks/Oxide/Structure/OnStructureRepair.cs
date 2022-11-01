///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Hooks
{
	[OxideHook("OnStructureRepair", typeof(object)), OxideHook.Category(Hook.Category.Enum.Structure)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("hitInfo", typeof(HitInfo))]
	[OxideHook.Info("Called when the player repairs a BuildingBlock or BaseCombatEntity.")]
	[OxideHook.Patch(typeof(BaseCombatEntity), "DoRepair")]
	public class BaseCombatEntity_DoRepair
	{
		public static bool Prefix(BasePlayer player, ref BaseCombatEntity __instance)
		{
			if (!__instance.repair.enabled) return false;

			return HookCaller.CallStaticHook("OnHammerHit", __instance, player) == null;
		}
	}
}
