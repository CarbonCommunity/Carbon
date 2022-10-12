///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnSupplyDropDropped", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("supplyDrop", typeof(SupplyDrop))]
	[OxideHook.Parameter("cargoPlane", typeof(CargoPlane))]
	[OxideHook.Info("Called right after a cargo plane has dropped a supply drop.")]
	[OxideHook.Patch(typeof(CargoPlane), "Update")]
	public class CargoPlane_Update
	{
		public static bool Prefix(ref CargoPlane __instance)
		{
			if (!__instance.isServer)
			{
				return false;
			}
			__instance.secondsTaken += Time.deltaTime;
			var num = Mathf.InverseLerp(0f, __instance.secondsToTake, __instance.secondsTaken);
			if (!__instance.dropped && num >= 0.5f)
			{
				__instance.dropped = true;
				var baseEntity = GameManager.server.CreateEntity(__instance.prefabDrop.resourcePath, __instance.transform.position, default, true);
				if (baseEntity)
				{
					baseEntity.globalBroadcast = true;
					baseEntity.Spawn();
					Interface.CallHook("OnSupplyDropDropped", baseEntity, __instance);
				}
			}
			__instance.transform.position = Vector3.Lerp(__instance.startPos, __instance.endPos, num);
			__instance.transform.hasChanged = true;
			if (num >= 1f)
			{
				__instance.Kill(BaseNetworkable.DestroyMode.None);
			}
			return false;
		}
	}
}
