///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Rust;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnFireBallDamage"), OxideHook.Category(Hook.Category.Enum.Weapon)]
	[OxideHook.Parameter("fire", typeof(FireBall))]
	[OxideHook.Parameter("entity", typeof(BaseCombatEntity))]
	[OxideHook.Parameter("info", typeof(HitInfo))]
	[OxideHook.Info("Called when a fire ball does damage to another entity.")]
	[OxideHook.Patch(typeof(FireBall), "DoRadialDamage")]
	public class FireBall_DoRadialDamage
	{
		public static bool Prefix(ref FireBall __instance)
		{
			var list = Facepunch.Pool.GetList<Collider>();
			Vector3 position = __instance.transform.position + new Vector3(0.0f, __instance.radius * 0.75f, 0.0f);
			Vis.Colliders(position, __instance.radius, list, (int)__instance.AttackLayers);
			var info = new HitInfo
			{
				DoHitEffects = true,
				DidHit = true,
				HitBone = 0U,
				Initiator = __instance.creatorEntity == null ? __instance.gameObject.ToBaseEntity() : __instance.creatorEntity,
				PointStart = __instance.transform.position
			};
			foreach (var collider in list)
			{
				if (!collider.isTrigger || collider.gameObject.layer != 29 && collider.gameObject.layer != 18)
				{
					var baseEntity = collider.gameObject.ToBaseEntity() as BaseCombatEntity;

					if (!(baseEntity == null) && baseEntity.isServer && baseEntity.IsAlive() && (!__instance.ignoreNPC || !baseEntity.IsNpc) && baseEntity.IsVisible(position))
					{
						if (baseEntity is BasePlayer)
							Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseEntity, 0U, new Vector3(0.0f, 1f, 0.0f), Vector3.up);

						info.PointEnd = baseEntity.transform.position;
						info.HitPositionWorld = baseEntity.transform.position;
						info.damageTypes.Set(DamageType.Heat, __instance.damagePerSecond * __instance.tickRate);
						HookCaller.CallStaticHook("OnFireBallDamage", __instance, baseEntity, info);
						baseEntity.OnAttacked(info);
					}
				}
			}
			Facepunch.Pool.FreeList(ref list);
			return false;
		}
	}
}
