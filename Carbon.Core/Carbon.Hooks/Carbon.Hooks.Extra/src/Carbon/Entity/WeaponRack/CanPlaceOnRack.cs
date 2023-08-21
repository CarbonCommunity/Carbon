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
		[HookAttribute.Patch("CanPlaceOnRack", "CanPlaceOnRack", typeof(WeaponRack), "InventoryItemFilter", new System.Type[] { typeof(Item), typeof(int) })]
		[HookAttribute.Identifier("68f53adc59a14185a3d6d518cfe64e3f")]

		[MetadataAttribute.Info("Returning a non-null value disallows the weapon to be placed.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("item", typeof(Item))]
		[MetadataAttribute.Return(typeof(bool))]

		public class Entity_WeaponRack_68f53adc59a14185a3d6d518cfe64e3f : Patch
		{
			public static bool Prefix(Item item, int targetSlot, WeaponRack __instance, ref bool __result)
			{
				if (HookCaller.CallStaticHook(2507203607, __instance, item) != null)
				{
					__result = false;
					return false;
				}

				return true;
			}
		}
	}
}
