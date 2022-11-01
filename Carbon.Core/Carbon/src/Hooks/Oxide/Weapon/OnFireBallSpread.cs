///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnFireBallSpread"), OxideHook.Category(Hook.Category.Enum.Weapon)]
	[OxideHook.Parameter("ball", typeof(FireBall))]
	[OxideHook.Parameter("fire", typeof(BaseEntity))]
	[OxideHook.Info("Called when a fire ball fire spreads.")]
	[OxideHook.Patch(typeof(FireBall), "TryToSpread")]
	public class FireBall_TryToSpread
	{
		public static bool Prefix(ref FireBall __instance)
		{
			var num = 0.9f - __instance.generation * 0.1f;

			if ((double)Random.Range(0.0f, 1f) >= (double)num || !__instance.spreadSubEntity.isValid)
				return false;

			var entity = GameManager.server.CreateEntity(__instance.spreadSubEntity.resourcePath);

			if (!(bool)entity)
				return false;

			entity.transform.position = __instance.transform.position + Vector3.up * 0.25f;
			entity.Spawn();

			var aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.up);
			entity.creatorEntity = __instance.creatorEntity == null ? entity : __instance.creatorEntity;

			HookCaller.CallStaticHook("OnFireBallSpread", __instance, entity);

			entity.SetVelocity(aimConeDirection * Random.Range(5f, 8f));
			entity.SendMessage("SetGeneration", __instance.generation + 1.0f);
			return false;
		}
	}
}
