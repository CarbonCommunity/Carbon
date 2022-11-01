///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnAmmoSwitch", typeof(object)), OxideHook.Category(Hook.Category.Enum.Weapon)]
	[OxideHook.Parameter("weapon", typeof(BaseProjectile))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player starts to switch the ammo in a weapon.")]
	[OxideHook.Patch(typeof(BaseProjectile), "SwitchAmmoTo")]
	public class BaseProjectile_SwitchAmmoTo
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref BaseProjectile __instance)
		{
			var ownerPlayer = __instance.GetOwnerPlayer();
			if (!ownerPlayer)
			{
				return false;
			}

			var num = msg.read.Int32();
			if (num == __instance.primaryMagazine.ammoType.itemid)
			{
				return false;
			}

			var itemDefinition = ItemManager.FindItemDefinition(num);
			if (itemDefinition == null)
			{
				return false;
			}

			var component = itemDefinition.GetComponent<ItemModProjectile>();
			if (!component || !component.IsAmmo(__instance.primaryMagazine.definition.ammoTypes))
			{
				return false;
			}

			return Interface.CallHook("OnAmmoSwitch", __instance, ownerPlayer) == null;
		}
	}
}
