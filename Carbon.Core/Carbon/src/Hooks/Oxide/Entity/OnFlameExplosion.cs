///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnFlameExplosion", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("explosive", typeof(FlameExplosive))]
	[OxideHook.Parameter("flame", typeof(BaseEntity))]
	[OxideHook.Info("Called when a flame explodes.")]
	[OxideHook.Patch(typeof(FlameExplosive), "FlameExplode")]
	public class FlameExplosive_FlameExplode
	{
		public static bool Prefix(Vector3 surfaceNormal, ref FlameExplosive __instance)
		{
			if (!__instance.isServer)
			{
				return false;
			}

			var num = 0;
			while (num < __instance.numToCreate)
			{
				var baseEntity = GameManager.server.CreateEntity(__instance.createOnExplode.resourcePath, __instance.transform.position, default(Quaternion), true);
				if (baseEntity)
				{
					var modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(__instance.spreadAngle, surfaceNormal, true);
					baseEntity.transform.SetPositionAndRotation(__instance.transform.position, Quaternion.LookRotation(modifiedAimConeDirection));
					baseEntity.creatorEntity = ((__instance.creatorEntity == null) ? baseEntity : __instance.creatorEntity);

					Interface.CallHook("OnFlameExplosion", __instance, baseEntity);

					baseEntity.Spawn();
					baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(__instance.minVelocity, __instance.maxVelocity));
				}
				num++;
			}

			return false;
		}
	}
}
