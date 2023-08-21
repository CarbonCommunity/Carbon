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
		[HookAttribute.Patch("CanPickupAllFromRack", "CanPickupAllFromRack", typeof(WeaponRack), "GivePlayerAllWeapons", new System.Type[] { typeof(BasePlayer), typeof(int) })]
		[HookAttribute.Identifier("c1c6e670372c4566842af773ef81b971")]

		[MetadataAttribute.Info("Returning a non-null value disallows all weapons to be picked up from the rack.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("slot", typeof(WeaponRackSlot))]
		[MetadataAttribute.Return(typeof(bool))]

		public class Entity_WeaponRack_c1c6e670372c4566842af773ef81b971 : Patch
		{
			public static bool Prefix(BasePlayer player, int mountSlotIndex, WeaponRack __instance)
			{
				if (player == null)
				{
					return false;
				}

				var weaponAtIndex = __instance.GetWeaponAtIndex(mountSlotIndex);

				if (HookCaller.CallStaticHook(2195047299, __instance, weaponAtIndex) != null)
				{
					return false;
				}

				return true;
			}
		}
	}
}
