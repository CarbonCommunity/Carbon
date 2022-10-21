///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnFlameThrowerBurn"), OxideHook.Category(Hook.Category.Enum.Weapon)]
	[OxideHook.Parameter("thrower", typeof(FlameThrower))]
	[OxideHook.Parameter("entity", typeof(BaseEntity))]
	[OxideHook.Info("Called when the burn from a flame thrower spreads.")]
	[OxideHook.Patch(typeof(FlameThrower), "FlameTick")]
	public class FlameThrower_FlameTick
	{
		public static bool Prefix(ref FlameThrower __instance)
		{
			var num1 = Time.realtimeSinceStartup - __instance.lastFlameTick;
			__instance.lastFlameTick = Time.realtimeSinceStartup;
			var ownerPlayer = __instance.GetOwnerPlayer();
			if (!(bool)ownerPlayer)
				return false;
			__instance.ReduceAmmo(num1);
			__instance.SendNetworkUpdate();
			var ray = ownerPlayer.eyes.BodyRay();
			var origin = ray.origin;
			RaycastHit hitInfo;
			var num2 = Physics.SphereCast(ray, 0.3f, out hitInfo, __instance.flameRange, 1218652417) ? 1 : 0;
			if (num2 == 0)
				hitInfo.point = origin + ray.direction * __instance.flameRange;
			var num3 = ownerPlayer.IsNpc ? __instance.npcDamageScale : 1f;
			var amount = __instance.damagePerSec[0].amount;
			__instance.damagePerSec[0].amount = amount * num1 * num3;
			DamageUtil.RadiusDamage(ownerPlayer, __instance.LookupPrefab(), hitInfo.point - ray.direction * 0.1f, __instance.flameRadius * 0.5f, __instance.flameRadius, __instance.damagePerSec, 2279681, true);
			__instance.damagePerSec[0].amount = amount;
			if (num2 != 0 && (double)Time.realtimeSinceStartup >= __instance.nextFlameTime && (double)hitInfo.distance > 1.10000002384186)
			{
				__instance.nextFlameTime = Time.realtimeSinceStartup + 0.45f;
				var point = hitInfo.point;
				var entity = GameManager.server.CreateEntity(__instance.fireballPrefab.resourcePath, point - ray.direction * 0.25f);
				if ((bool)entity)
				{
					HookCaller.CallStaticHook("OnFlameThrowerBurn", __instance, entity);
					entity.creatorEntity = ownerPlayer;
					entity.Spawn();
				}
			}
			if (__instance.ammo == 0)
				__instance.SetFlameState(false);
			__instance.GetOwnerItem()?.LoseCondition(num1);
			return false;
		}
	}
}
