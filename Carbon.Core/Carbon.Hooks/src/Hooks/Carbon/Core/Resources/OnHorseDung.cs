using Oxide.Core;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Resources
{
	public partial class Resources_BaseRidableAnimal
	{
		/*
		[CarbonHook("OnHorseDung", typeof(Item)), Hook.Category(Hook.Category.Enum.Entity)]
		[Hook.Parameter("horse", typeof(BaseRidableAnimal))]
		[Hook.Info("Called when a dung is spawned at the backside of the horse.")]
		[Hook.Info("Override the return with an item to replace the Dung with any other item that's being dropped.")]
		[Hook.Patch(typeof(BaseRidableAnimal), "DoDung")]
		*/

		public class Resources_BaseRidableAnimal_DoDung_e5beabfdc524496dbf5657149585bdac
		{
			public static Metadata metadata = new Metadata("OnHorseDung",
				typeof(BaseRidableAnimal), "DoDung", new System.Type[] { });

			static Resources_BaseRidableAnimal_DoDung_e5beabfdc524496dbf5657149585bdac()
			{
				metadata.SetIdentifier("e5beabfdc524496dbf5657149585bdac");
			}

			public static bool Prefix(ref BaseRidableAnimal __instance)
			{
				var result = Interface.CallHook("OnHorseDung", __instance);
				var dungItem = (Item)null;

				if (result is Item)
				{
					dungItem = result as Item;
				}

				if (dungItem == null) dungItem = ItemManager.Create(__instance.Dung, 1, 0uL);

				__instance.dungProduction -= 1f;
				dungItem.Drop(__instance.transform.position + -__instance.transform.forward + Vector3.up * 1.1f + Random.insideUnitSphere * 0.1f, -__instance.transform.forward);
				return false;
			}
		}
	}
}
