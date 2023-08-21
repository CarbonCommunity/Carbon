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
	public partial class WeaponRack_Entity
	{
		[HookAttribute.Patch("CanPickupFromRack", "CanPickupFromRack", typeof(WeaponRack), "GivePlayerWeapon", new System.Type[] { typeof(BasePlayer), typeof(int), typeof(int), typeof(bool), typeof(bool) })]
		[HookAttribute.Identifier("ca34645d39b24c328b84ce4efd4fdb34")]

		[MetadataAttribute.Info("Returning a non-null value disallows the weapon to be picked up.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("item", typeof(Item))]
		[MetadataAttribute.Return(typeof(bool))]

		public class Entity_WeaponRack_ca34645d39b24c328b84ce4efd4fdb34 : Patch
		{
			public static bool Prefix(BasePlayer player, int mountSlotIndex, int playerBeltIndex, bool tryHold, bool sendUpdate, WeaponRack __instance)
			{
				if (player == null)
				{
					return false;
				}

				var weaponAtIndex = __instance.GetWeaponAtIndex(mountSlotIndex);

				if (weaponAtIndex == null)
				{
					return false;
				}

				var slot = __instance.inventory.GetSlot(weaponAtIndex.InventoryIndex);

				if (slot == null)
				{
					return false;
				}

				if (HookCaller.CallStaticHook(2308599075, __instance, weaponAtIndex, slot) != null)
				{
					return false;
				}

				return true;
			}
		}
	}
}
