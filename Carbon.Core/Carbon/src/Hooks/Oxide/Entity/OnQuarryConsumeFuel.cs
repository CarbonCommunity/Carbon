///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnQuarryConsumeFuel", typeof(Item)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("entity", typeof(BaseNetworkable))]
	[OxideHook.Info("Used to override the fuel needed for any Quarries & Pumpjacks that are running on a server.")]
	[OxideHook.Patch(typeof(MiningQuarry), "FuelCheck")]
	public class MiningQuarry_FuelCheck
	{
		public static bool Prefix(ref MiningQuarry __instance, out bool __result)
		{
			if (__instance.pendingWork > 0f)
			{
				__result = true;
				return false;
			}

			var fuel = __instance.fuelStoragePrefab.instance as StorageContainer;
			var item = fuel.inventory.FindItemsByItemName("diesel_barrel"); // Default behaviour
			var obj = Interface.CallHook("OnQuarryConsumeFuel", __instance, item);

			if (obj is Item itemObj)
			{
				item = itemObj;
			}

			if (item != null && item.amount >= 1)
			{
				__instance.pendingWork += __instance.workPerFuel;
				item.UseItem(1);
				__result = true;
				return false;
			}

			__result = false;
			return false;
		}
	}
}
