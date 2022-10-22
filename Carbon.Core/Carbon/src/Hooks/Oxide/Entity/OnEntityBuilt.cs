///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.ComponentModel;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnEntityBuilt", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("planner", typeof(Planner))]
	[OxideHook.Parameter("gameObject", typeof(GameObject))]
	[OxideHook.Info("Called when any structure is built (walls, ceilings, stairs, etc.).")]
	[OxideHook.Patch(typeof(Planner), "DoBuild")]
	public class Planner_DoBuild_OnEntityBuilt
	{
		public static bool Prefix(Construction.Target target, Construction component, ref Planner __instance)
		{
			var ownerPlayer = __instance.GetOwnerPlayer();

			if (!ownerPlayer)
			{
				return false;
			}
			if (target.ray.IsNaNOrInfinity())
			{
				return false;
			}
			if (Vector3Ex.IsNaNOrInfinity(target.position))
			{
				return false;
			}
			if (Vector3Ex.IsNaNOrInfinity(target.normal))
			{
				return false;
			}

			if (target.socket != null)
			{
				if (!target.socket.female)
				{
					ownerPlayer.ChatMessage("Target socket is not female. (" + target.socket.socketName + ")");
					return false;
				}
				if (target.entity != null && target.entity.IsOccupied(target.socket))
				{
					ownerPlayer.ChatMessage("Target socket is occupied. (" + target.socket.socketName + ")");
					return false;
				}
				if (target.onTerrain)
				{
					ownerPlayer.ChatMessage("Target on terrain is not allowed when attaching to socket. (" + target.socket.socketName + ")");
					return false;
				}
			}

			if (ConVar.AntiHack.eye_protection >= 2)
			{
				var center = ownerPlayer.eyes.center;
				var position = ownerPlayer.eyes.position;
				var origin = target.ray.origin;
				var vector = (target.entity != null && target.socket != null) ? target.GetWorldPosition() : target.position;
				var num = 2097152;
				var num2 = ConVar.AntiHack.build_terraincheck ? 10551296 : 2162688;
				var num3 = ConVar.AntiHack.build_losradius;
				var padding = ConVar.AntiHack.build_losradius + 0.01f;
				var layerMask = num2;

				if (target.socket != null)
				{
					num3 = 0f;
					padding = 0.5f;
					layerMask = num;
				}
				if (component.isSleepingBag)
				{
					num3 = ConVar.AntiHack.build_losradius_sleepingbag;
					padding = ConVar.AntiHack.build_losradius_sleepingbag + 0.01f;
					layerMask = num2;
				}
				if (num3 > 0f)
				{
					vector += target.normal.normalized * num3;
				}
				if (target.entity != null)
				{
					DeployShell deployShell = PrefabAttribute.server.Find<DeployShell>(target.entity.prefabID);
					if (deployShell != null)
					{
						vector += target.normal.normalized * deployShell.LineOfSightPadding();
					}
				}
				if (!GamePhysics.LineOfSightRadius(center, position, layerMask, num3, null) || !GamePhysics.LineOfSightRadius(position, origin, layerMask, num3, null) || !GamePhysics.LineOfSightRadius(origin, vector, layerMask, num3, 0f, padding, null))
				{
					ownerPlayer.ChatMessage("Line of sight blocked.");
					return false;
				}
			}
			Construction.lastPlacementError = "No Error";

			var gameObject = __instance.DoPlacement(target, component);
			if (gameObject == null)
			{
				ownerPlayer.ChatMessage("Can't place: " + Construction.lastPlacementError);
			}
			if (gameObject != null)
			{
				Interface.CallHook("OnEntityBuilt", __instance, gameObject);

				var deployable = __instance.GetDeployable();
				if (deployable != null)
				{
					var baseEntity = gameObject.ToBaseEntity();
					if (deployable.setSocketParent && target.entity != null && target.entity.SupportsChildDeployables() && baseEntity)
					{
						baseEntity.SetParent(target.entity, true, false);
					}
					if (deployable.wantsInstanceData && __instance.GetOwnerItem().instanceData != null)
					{
						(baseEntity as IInstanceDataReceiver).ReceiveInstanceData(__instance.GetOwnerItem().instanceData);
					}
					if (deployable.copyInventoryFromItem)
					{
						var component2 = baseEntity.GetComponent<StorageContainer>();
						if (component2)
						{
							component2.ReceiveInventoryFromItem(__instance.GetOwnerItem());
						}
					}
					var modDeployable = __instance.GetModDeployable();
					if (modDeployable != null)
					{
						modDeployable.OnDeployed(baseEntity, ownerPlayer);
					}
					baseEntity.OnDeployed(baseEntity.GetParentEntity(), ownerPlayer, __instance.GetOwnerItem());
					if (deployable.placeEffect.isValid)
					{
						if (target.entity && target.socket != null)
						{
							Effect.server.Run(deployable.placeEffect.resourcePath, target.entity.transform.TransformPoint(target.socket.worldPosition), target.entity.transform.up, null, false);
						}
						else
						{
							Effect.server.Run(deployable.placeEffect.resourcePath, target.position, target.normal, null, false);
						}
					}
				}

				__instance.PayForPlacement(ownerPlayer, component);
			}

			return false;
		}
	}
}
