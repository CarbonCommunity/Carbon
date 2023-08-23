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
		[HookAttribute.Patch("OnPickUpFromRack", "OnPickUpFromRack", typeof(WeaponRack), "InventoryItemFilter", new System.Type[] { typeof(Item), typeof(int) })]
		[HookAttribute.Identifier("218d76c79e534841b0eacf66932f1769")]

		[MetadataAttribute.Info("Whenever a weapon was picked up from the rack.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("item", typeof(Item))]
		[MetadataAttribute.Parameter("targetSlot", typeof(int))]

		public class Entity_WeaponRack_218d76c79e534841b0eacf66932f1769 : Patch
		{
			public static void Postfix(Item item, int targetSlot, WeaponRack __instance)
			{
				HookCaller.CallStaticHook(1687007383, __instance, item, targetSlot);
			}
		}
	}
}
