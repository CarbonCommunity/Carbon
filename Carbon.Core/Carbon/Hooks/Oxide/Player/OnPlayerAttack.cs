///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using ProtoBuf;
using UnityEngine;

namespace Carbon.Extended
{
	[OxideHook("OnPlayerAttack", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("hitInfo", typeof(HitInfo))]
	[OxideHook.Info("Useful for modifying an attack before it goes out.")]
	[OxideHook.Info("hitInfo.HitEntity should be the entity that this attack would hit.")]
	[OxideHook.Patch(typeof(BaseMelee), "DoAttackShared")]
	public class BaseMelee_DoAttackShared
	{
		public static bool Prefix(HitInfo info, ref BaseMelee __instance)
		{
			return Interface.CallHook("OnPlayerAttack", __instance.GetOwnerPlayer(), info) == null;
		}
	}

	[OxideHook("OnPlayerAttack", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("hitInfo2", typeof(HitInfo))]
	[OxideHook.Info("Useful for modifying an attack before it goes out.")]
	[OxideHook.Info("hitInfo.HitEntity should be the entity that this attack would hit.")]
	[OxideHook.Patch(typeof(BasePlayer), "OnProjectileAttack")]
	public class BasePlayer_OnProjectileAttack
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref BasePlayer __instance)
		{
			Core.CarbonCore.Log($"Test {__instance}");

			PlayerProjectileAttack playerProjectileAttack = PlayerProjectileAttack.Deserialize(msg.read);
			if (playerProjectileAttack == null)
			{
				return false;
			}
			PlayerAttack playerAttack = playerProjectileAttack.playerAttack;
			HitInfo hitInfo = new HitInfo();
			hitInfo.LoadFromAttack(playerAttack.attack, true);
			hitInfo.Initiator = __instance;
			hitInfo.ProjectileID = playerAttack.projectileID;
			hitInfo.ProjectileDistance = playerProjectileAttack.hitDistance;
			hitInfo.ProjectileVelocity = playerProjectileAttack.hitVelocity;
			hitInfo.Predicted = msg.connection;
			if (hitInfo.IsNaNOrInfinity() || float.IsNaN(playerProjectileAttack.travelTime) || float.IsInfinity(playerProjectileAttack.travelTime))
			{
				AntiHack.Log(__instance, AntiHackType.ProjectileHack, "Contains NaN (" + playerAttack.projectileID + ")");
				playerProjectileAttack.ResetToPool();
				__instance.stats.combat.LogInvalid(hitInfo, "projectile_nan");
				return false;
			}
			BasePlayer.FiredProjectile firedProjectile;
			if (!__instance.firedProjectiles.TryGetValue(playerAttack.projectileID, out firedProjectile))
			{
				AntiHack.Log(__instance, AntiHackType.ProjectileHack, "Missing ID (" + playerAttack.projectileID + ")");
				playerProjectileAttack.ResetToPool();
				__instance.stats.combat.LogInvalid(hitInfo, "projectile_invalid");
				return false;
			}
			hitInfo.ProjectileHits = firedProjectile.hits;
			hitInfo.ProjectileIntegrity = firedProjectile.integrity;
			hitInfo.ProjectileTravelTime = firedProjectile.travelTime;
			hitInfo.ProjectileTrajectoryMismatch = firedProjectile.trajectoryMismatch;
			if (firedProjectile.integrity <= 0f)
			{
				AntiHack.Log(__instance, AntiHackType.ProjectileHack, "Integrity is zero (" + playerAttack.projectileID + ")");
				playerProjectileAttack.ResetToPool();
				__instance.stats.combat.LogInvalid(hitInfo, "projectile_integrity");
				return false;
			}
			if (firedProjectile.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
			{
				AntiHack.Log(__instance, AntiHackType.ProjectileHack, "Lifetime is zero (" + playerAttack.projectileID + ")");
				playerProjectileAttack.ResetToPool();
				__instance.stats.combat.LogInvalid(hitInfo, "projectile_lifetime");
				return false;
			}
			if (firedProjectile.ricochets > 0)
			{
				AntiHack.Log(__instance, AntiHackType.ProjectileHack, "Projectile is ricochet (" + playerAttack.projectileID + ")");
				playerProjectileAttack.ResetToPool();
				__instance.stats.combat.LogInvalid(hitInfo, "projectile_ricochet");
				return false;
			}
			hitInfo.Weapon = firedProjectile.weaponSource;
			hitInfo.WeaponPrefab = firedProjectile.weaponPrefab;
			hitInfo.ProjectilePrefab = firedProjectile.projectilePrefab;
			hitInfo.damageProperties = firedProjectile.projectilePrefab.damageProperties;
			Vector3 position = firedProjectile.position;
			Vector3 velocity = firedProjectile.velocity;
			float partialTime = firedProjectile.partialTime;
			float travelTime = firedProjectile.travelTime;
			float num = Mathf.Clamp(playerProjectileAttack.travelTime, firedProjectile.travelTime, 8f);
			Vector3 gravity = UnityEngine.Physics.gravity * firedProjectile.projectilePrefab.gravityModifier;
			float drag = firedProjectile.projectilePrefab.drag;
			int layerMask = ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688;
			BaseEntity hitEntity = hitInfo.HitEntity;
			BasePlayer basePlayer = hitEntity as BasePlayer;
			bool flag = basePlayer != null;
			bool flag2 = flag && basePlayer.IsSleeping();
			bool flag3 = flag && basePlayer.IsWounded();
			bool flag4 = flag && basePlayer.isMounted;
			bool flag5 = flag && basePlayer.HasParent();
			bool flag6 = hitEntity != null;
			bool flag7 = flag6 && hitEntity.IsNpc;
			bool flag8 = hitInfo.HitMaterial == Projectile.WaterMaterialID();
			if (firedProjectile.protection > 0)
			{
				bool flag9 = true;
				float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
				float projectile_clientframes = ConVar.AntiHack.projectile_clientframes;
				float projectile_serverframes = ConVar.AntiHack.projectile_serverframes;
				float num3 = Mathx.Decrement(firedProjectile.firedTime);
				float num4 = Mathf.Clamp(Mathx.Increment(UnityEngine.Time.realtimeSinceStartup) - num3, 0f, 8f);
				float num5 = num;
				float num6 = Mathf.Abs(num4 - num5);
				firedProjectile.desyncLifeTime = num6;
				float num7 = Mathf.Min(num4, num5);
				float num8 = projectile_clientframes / 60f;
				float num9 = projectile_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
				float num10 = (__instance.desyncTimeClamped + num7 + num8 + num9) * num2;
				float num11 = (firedProjectile.protection >= 6) ? ((__instance.desyncTimeClamped + num8 + num9) * num2) : num10;
				if (flag && hitInfo.boneArea == (global::HitArea)-1)
				{
					string name = hitInfo.ProjectilePrefab.name;
					string text = flag6 ? hitEntity.ShortPrefabName : "world";
					AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
					"Bone is invalid (",
					name,
					" on ",
					text,
					" bone ",
					hitInfo.HitBone,
					")"
					}));
					__instance.stats.combat.LogInvalid(hitInfo, "projectile_bone");
					flag9 = false;
				}
				if (flag8)
				{
					if (flag6)
					{
						string name2 = hitInfo.ProjectilePrefab.name;
						string text2 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new string[]
						{
						"Projectile water hit on entity (",
						name2,
						" on ",
						text2,
						")"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "water_entity");
						flag9 = false;
					}
					if (!WaterLevel.Test(hitInfo.HitPositionWorld - 0.5f * Vector3.up, false, __instance))
					{
						string name3 = hitInfo.ProjectilePrefab.name;
						string text3 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new string[]
						{
						"Projectile water level (",
						name3,
						" on ",
						text3,
						")"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "water_level");
						flag9 = false;
					}
				}
				if (firedProjectile.protection >= 2)
				{
					if (flag6)
					{
						float num12 = hitEntity.MaxVelocity() + hitEntity.GetParentVelocity().magnitude;
						float num13 = hitEntity.BoundsPadding() + num11 * num12;
						float num14 = hitEntity.Distance(hitInfo.HitPositionWorld);
						if (num14 > num13)
						{
							string name4 = hitInfo.ProjectilePrefab.name;
							string shortPrefabName = hitEntity.ShortPrefabName;
							AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
							{
							"Entity too far away (",
							name4,
							" on ",
							shortPrefabName,
							" with ",
							num14,
							"m > ",
							num13,
							"m in ",
							num11,
							"s)"
							}));
							__instance.stats.combat.LogInvalid(hitInfo, "entity_distance");
							flag9 = false;
						}
					}
					if (firedProjectile.protection >= 6 && flag9 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
					{
						float magnitude = basePlayer.GetParentVelocity().magnitude;
						float num15 = basePlayer.BoundsPadding() + num11 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
						float num16 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
						if (num16 > num15)
						{
							string name5 = hitInfo.ProjectilePrefab.name;
							string shortPrefabName2 = basePlayer.ShortPrefabName;
							AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
							{
							"Player too far away (",
							name5,
							" on ",
							shortPrefabName2,
							" with ",
							num16,
							"m > ",
							num15,
							"m in ",
							num11,
							"s)"
							}));
							__instance.stats.combat.LogInvalid(hitInfo, "player_distance");
							flag9 = false;
						}
					}
				}
				if (firedProjectile.protection >= 1)
				{
					float magnitude2 = firedProjectile.initialVelocity.magnitude;
					float num17 = hitInfo.ProjectilePrefab.initialDistance + num10 * magnitude2;
					float num18 = hitInfo.ProjectileDistance + 1f;
					float num19 = Vector3.Distance(firedProjectile.initialPosition, hitInfo.HitPositionWorld);
					if (num19 > num17)
					{
						string name6 = hitInfo.ProjectilePrefab.name;
						string text4 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
						"Projectile too fast (",
						name6,
						" on ",
						text4,
						" with ",
						num19,
						"m > ",
						num17,
						"m in ",
						num10,
						"s)"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "projectile_speed");
						flag9 = false;
					}
					if (num19 > num18)
					{
						string name7 = hitInfo.ProjectilePrefab.name;
						string text5 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
						"Projectile too far away (",
						name7,
						" on ",
						text5,
						" with ",
						num19,
						"m > ",
						num18,
						"m in ",
						num10,
						"s)"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "projectile_distance");
						flag9 = false;
					}
					if (num6 > ConVar.AntiHack.projectile_desync)
					{
						string name8 = hitInfo.ProjectilePrefab.name;
						string text6 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
						"Projectile desync (",
						name8,
						" on ",
						text6,
						" with ",
						num6,
						"s > ",
						ConVar.AntiHack.projectile_desync,
						"s)"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "projectile_desync");
						flag9 = false;
					}
				}
				if (firedProjectile.protection >= 3)
				{
					Vector3 position2 = firedProjectile.position;
					Vector3 pointStart = hitInfo.PointStart;
					Vector3 vector = hitInfo.HitPositionWorld;
					Vector3 vector2 = hitInfo.PositionOnRay(vector);
					if (!flag8 && !flag6)
					{
						vector += hitInfo.HitNormalWorld.normalized * 0.001f;
					}
					bool flag10 = GamePhysics.LineOfSight(position2, pointStart, layerMask, firedProjectile.lastEntityHit) && GamePhysics.LineOfSight(pointStart, vector2, layerMask, firedProjectile.lastEntityHit) && GamePhysics.LineOfSight(vector2, vector, layerMask, hitEntity);
					if (!flag10)
					{
						__instance.stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_indirect_los", 1, Stats.Server);
					}
					else
					{
						__instance.stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_direct_los", 1, Stats.Server);
					}
					if (!flag10)
					{
						string name9 = hitInfo.ProjectilePrefab.name;
						string text7 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
						"Line of sight (",
						name9,
						" on ",
						text7,
						") ",
						position2,
						" ",
						pointStart,
						" ",
						vector2,
						" ",
						vector
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "projectile_los");
						flag9 = false;
					}
					if (flag9 && flag && !flag7)
					{
						Vector3 hitPositionWorld = hitInfo.HitPositionWorld;
						Vector3 position3 = basePlayer.eyes.position;
						Vector3 vector3 = basePlayer.CenterPoint();
						float projectile_losforgiveness = ConVar.AntiHack.projectile_losforgiveness;
						bool flag11 = GamePhysics.LineOfSight(hitPositionWorld, position3, layerMask, 0f, projectile_losforgiveness, null) && GamePhysics.LineOfSight(position3, hitPositionWorld, layerMask, projectile_losforgiveness, 0f, null);
						if (!flag11)
						{
							flag11 = (GamePhysics.LineOfSight(hitPositionWorld, vector3, layerMask, 0f, projectile_losforgiveness, null) && GamePhysics.LineOfSight(vector3, hitPositionWorld, layerMask, projectile_losforgiveness, 0f, null));
						}
						if (!flag11)
						{
							string name10 = hitInfo.ProjectilePrefab.name;
							string text8 = flag6 ? hitEntity.ShortPrefabName : "world";
							AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
							{
							"Line of sight (",
							name10,
							" on ",
							text8,
							") ",
							hitPositionWorld,
							" ",
							position3,
							" or ",
							hitPositionWorld,
							" ",
							vector3
							}));
							__instance.stats.combat.LogInvalid(hitInfo, "projectile_los");
							flag9 = false;
						}
					}
				}
				if (firedProjectile.protection >= 4)
				{
					Vector3 vector4;
					Vector3 vector5;
					__instance.SimulateProjectile(ref position, ref velocity, ref partialTime, num - travelTime, gravity, drag, out vector4, out vector5);
					Vector3 vector6 = vector5 * 0.03125f;
					Line line = new Line(vector4 - vector6, position + vector6);
					float num20 = line.Distance(hitInfo.PointStart);
					float num21 = line.Distance(hitInfo.HitPositionWorld);
					if (num20 > ConVar.AntiHack.projectile_trajectory)
					{
						string name11 = firedProjectile.projectilePrefab.name;
						string text9 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
						"Start position trajectory (",
						name11,
						" on ",
						text9,
						" with ",
						num20,
						"m > ",
						ConVar.AntiHack.projectile_trajectory,
						"m)"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "trajectory_start");
						flag9 = false;
					}
					if (num21 > ConVar.AntiHack.projectile_trajectory)
					{
						string name12 = firedProjectile.projectilePrefab.name;
						string text10 = flag6 ? hitEntity.ShortPrefabName : "world";
						AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
						"End position trajectory (",
						name12,
						" on ",
						text10,
						" with ",
						num21,
						"m > ",
						ConVar.AntiHack.projectile_trajectory,
						"m)"
						}));
						__instance.stats.combat.LogInvalid(hitInfo, "trajectory_end");
						flag9 = false;
					}
					hitInfo.ProjectileVelocity = velocity;
					if (playerProjectileAttack.hitVelocity != Vector3.zero && velocity != Vector3.zero)
					{
						float num22 = Vector3.Angle(playerProjectileAttack.hitVelocity, velocity);
						float num23 = playerProjectileAttack.hitVelocity.magnitude / velocity.magnitude;
						if (num22 > ConVar.AntiHack.projectile_anglechange)
						{
							string name13 = firedProjectile.projectilePrefab.name;
							string text11 = flag6 ? hitEntity.ShortPrefabName : "world";
							AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
							{
							"Trajectory angle change (",
							name13,
							" on ",
							text11,
							" with ",
							num22,
							"deg > ",
							ConVar.AntiHack.projectile_anglechange,
							"deg)"
							}));
							__instance.stats.combat.LogInvalid(hitInfo, "angle_change");
							flag9 = false;
						}
						if (num23 > ConVar.AntiHack.projectile_velocitychange)
						{
							string name14 = firedProjectile.projectilePrefab.name;
							string text12 = flag6 ? hitEntity.ShortPrefabName : "world";
							AntiHack.Log(__instance, AntiHackType.ProjectileHack, string.Concat(new object[]
							{
							"Trajectory velocity change (",
							name14,
							" on ",
							text12,
							" with ",
							num23,
							" > ",
							ConVar.AntiHack.projectile_velocitychange,
							")"
							}));
							__instance.stats.combat.LogInvalid(hitInfo, "velocity_change");
							flag9 = false;
						}
					}
				}
				if (!flag9)
				{
					AntiHack.AddViolation(__instance, AntiHackType.ProjectileHack, ConVar.AntiHack.projectile_penalty);
					playerProjectileAttack.ResetToPool();
					return false;
				}
			}
			firedProjectile.position = hitInfo.HitPositionWorld;
			firedProjectile.velocity = playerProjectileAttack.hitVelocity;
			firedProjectile.travelTime = num;
			firedProjectile.partialTime = partialTime;
			firedProjectile.hits++;
			firedProjectile.lastEntityHit = hitEntity;
			hitInfo.ProjectilePrefab.CalculateDamage(hitInfo, firedProjectile.projectileModifier, firedProjectile.integrity);
			if (firedProjectile.integrity < 1f)
			{
				firedProjectile.integrity = 0f;
			}
			else if (flag8)
			{
				firedProjectile.integrity = Mathf.Clamp01(firedProjectile.integrity - 0.1f);
			}
			else if (hitInfo.ProjectilePrefab.penetrationPower <= 0f || !flag6)
			{
				firedProjectile.integrity = 0f;
			}
			else
			{
				float num24 = hitEntity.PenetrationResistance(hitInfo) / hitInfo.ProjectilePrefab.penetrationPower;
				firedProjectile.integrity = Mathf.Clamp01(firedProjectile.integrity - num24);
			}
			if (flag6)
			{
				__instance.stats.Add(firedProjectile.itemMod.category + "_hit_" + hitEntity.Categorize(), 1, Stats.Steam);
			}
			if (Interface.CallHook("OnPlayerAttack", __instance, hitInfo) != null)
			{
				return false;
			}
			if (firedProjectile.integrity <= 0f)
			{
				if (firedProjectile.hits <= 1)
				{
					firedProjectile.itemMod.ServerProjectileHit(hitInfo);
				}
				if (hitInfo.ProjectilePrefab.remainInWorld)
				{
					__instance.CreateWorldProjectile(hitInfo, firedProjectile.itemDef, firedProjectile.itemMod, hitInfo.ProjectilePrefab, firedProjectile.pickupItem);
				}
			}
			__instance.firedProjectiles[playerAttack.projectileID] = firedProjectile;
			if (flag6)
			{
				if (firedProjectile.hits <= 2)
				{
					hitEntity.OnAttacked(hitInfo);
				}
				else
				{
					__instance.stats.combat.LogInvalid(hitInfo, "ricochet");
				}
			}
			hitInfo.DoHitEffects = hitInfo.ProjectilePrefab.doDefaultHitEffects;
			Effect.server.ImpactEffect(hitInfo);
			playerProjectileAttack.ResetToPool();
			return false;
		}
	}
}
