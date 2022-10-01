///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnPlayerRevive", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("target", typeof(BasePlayer))]
	[OxideHook.Info("Called when the recover after reviving with a medical tool.")]
	[OxideHook.Info("Useful for canceling the reviving.")]
	[OxideHook.Patch(typeof(MedicalTool), "GiveEffectsTo")]
	public class MedicalTool_GiveEffectsTo
	{
		public static bool Prefix(BasePlayer player, ref MedicalTool __instance)
		{
			if (player == null) return true;

			var component = __instance.GetOwnerItemDefinition().GetComponent<ItemModConsumable>();

			if (component != null)
			{
				return true;
			}

			var ownerPlayer = __instance.GetOwnerPlayer();

			if (player != ownerPlayer && player.IsWounded() && __instance.canRevive)
			{
				return Interface.CallHook("OnPlayerRevive", __instance.GetOwnerPlayer(), player) == null;
			}

			return true;
		}
	}

	[OxideHook("OnHealingItemUse", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("this", typeof(MedicalTool))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player attempts to use a medical tool.")]
	[OxideHook.Patch(typeof(MedicalTool), "GiveEffectsTo")]
	public class MedicalTool_GiveEffectsTo_OnHealingItemUse
	{
		public static bool Prefix(BasePlayer player, ref MedicalTool __instance)
		{
			if (player == null) return true;

			var component = __instance.GetOwnerItemDefinition().GetComponent<ItemModConsumable>();

			if (component != null)
			{
				return true;
			}

			if (Interface.CallHook("OnHealingItemUse", __instance, player) != null)
			{
				return false;
			}

			return true;
		}
	}
}
