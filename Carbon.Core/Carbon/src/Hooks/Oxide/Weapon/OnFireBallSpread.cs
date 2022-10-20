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
			float num = (float)(0.899999976158142 - (double)__instance.generation * 0.100000001490116);
			if ((double)UnityEngine.Random.Range(0.0f, 1f) >= (double)num || !__instance.spreadSubEntity.isValid)
				return false;
			var entity = GameManager.server.CreateEntity(__instance.spreadSubEntity.resourcePath);
			if (!(bool)entity)
				return false;
			entity.transform.position = __instance.transform.position + Vector3.up * 0.25f;
			entity.Spawn();
			Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.up);
			entity.creatorEntity = __instance.creatorEntity == null ? entity : __instance.creatorEntity;
			HookCaller.CallStaticHook("OnFireBallSpread", __instance, entity);
			entity.SetVelocity(aimConeDirection * UnityEngine.Random.Range(5f, 8f));
			entity.SendMessage("SetGeneration", (object)(float)((double)__instance.generation + 1.0));
			return false;
		}
	}
}
