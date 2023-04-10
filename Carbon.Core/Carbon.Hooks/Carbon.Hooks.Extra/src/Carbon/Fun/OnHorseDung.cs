using API.Hooks;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Resources
{
	public partial class Resources_BaseRidableAnimal
	{
		[HookAttribute.Patch("OnHorseDung", "OnHorseDung", typeof(BaseRidableAnimal), "DoDung", new System.Type[] { })]
		[HookAttribute.Identifier("e5beabfdc524496dbf5657149585bdac")]

		[MetadataAttribute.Parameter("horse", typeof(BaseRidableAnimal))]
		[MetadataAttribute.Info("Called when a dung is spawned at the backside of the horse.")]
		[MetadataAttribute.Info("Override the return with an item to replace the Dung with any other item that's being dropped.")]

		public class Resources_BaseRidableAnimal_e5beabfdc524496dbf5657149585bdac : Patch
		{
			public static bool Prefix(ref BaseRidableAnimal __instance)
			{
				var result = HookCaller.CallStaticHook("OnHorseDung", __instance);
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
