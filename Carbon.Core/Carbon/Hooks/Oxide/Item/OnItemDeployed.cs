///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Core.Hooks.Oxide.Item
{
    [OxideHook( "OnItemDeployed"), OxideHook.Category(Hook.Category.Enum.Item)]
    [OxideHook.Parameter("deployer", typeof(Deployer))]
    [OxideHook.Parameter("baseEntity", typeof(BaseEntity))]
    [OxideHook.Parameter("slotEntity", typeof(BaseEntity))]
    [OxideHook.Info( "Called right after an item has been deployed." )]
    [OxideHook.Patch(typeof(Deployer), "DoDeploy_Regular" )]
    public class Deployer_OnItemDeployed
    {
        public static bool Prefix(Deployable deployable, Ray ray, ref Deployer __instance)
        {
            if ( !__instance.HasItemAmount () )
            {
                return false;
            }

            var ownerPlayer = __instance.GetOwnerPlayer ();

            if ( !ownerPlayer )
            {
                return false;
            }
            if ( !ownerPlayer.CanBuild () )
            {
                ownerPlayer.ChatMessage ( "Building is blocked at player position!" );
                return false;
            }
            if ( ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack ( 1f, float.PositiveInfinity ) )
            {
                ownerPlayer.ChatMessage ( "AntiHack!" );
                return false;
            }
            if ( !__instance.CheckPlacement ( deployable, ray, 8f ) )
            {
                return false;
            }
            RaycastHit raycastHit;

            if ( !UnityEngine.Physics.Raycast ( ray, out raycastHit, 8f, 1235288065 ) )
            {
                return false;
            }

            var point = raycastHit.point;
            var deployedRotation = __instance.GetDeployedRotation ( raycastHit.normal, ray.direction );
            var ownerItem = __instance.GetOwnerItem ();
            var  modDeployable = __instance.GetModDeployable ();
            if ( ownerPlayer.Distance ( point ) > 3f )
            {
                ownerPlayer.ChatMessage ( "Too far away!" );
                return false;
            }
            if ( !ownerPlayer.CanBuild ( point, deployedRotation, deployable.bounds ) )
            {
                ownerPlayer.ChatMessage ( "Building is blocked at placement position!" );
                return false;
            }
            BaseEntity baseEntity = GameManager.server.CreateEntity ( modDeployable.entityPrefab.resourcePath, point, deployedRotation, true );
            if ( !baseEntity )
            {
                Debug.LogWarning ( "Couldn't create prefab:" + modDeployable.entityPrefab.resourcePath );
                return false;
            }
            baseEntity.skinID = ownerItem.skin;
            baseEntity.SendMessage ( "SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver );
            baseEntity.OwnerID = ownerPlayer.userID;
            baseEntity.Spawn ();
            modDeployable.OnDeployed ( baseEntity, ownerPlayer );
            Interface.CallHook ( "OnItemDeployed", __instance, modDeployable, baseEntity );
            __instance.UseItemAmount ( 1 );
            return false;
        }
    }

    [OxideHook ( "OnItemDeployed" ), OxideHook.Category ( Hook.Category.Enum.Item )]
    [OxideHook.Parameter ( "deployer", typeof ( Deployer ) )]
    [OxideHook.Parameter ( "baseEntity", typeof ( BaseEntity ) )]
    [OxideHook.Parameter ( "slotEntity", typeof ( BaseEntity ) )]
    [OxideHook.Info ( "Called right after an item has been deployed." )]
    [OxideHook.Patch ( typeof ( Deployer ), "DoDeploy_Slot" )]
    public class Deployer_DoDeploy_Slot
    {
        public static bool Prefix ( Deployable deployable, Ray ray, uint entityID, ref Deployer __instance )
        {
            if ( !__instance.HasItemAmount () )
            {
                return false;
            }

            var ownerPlayer = __instance.GetOwnerPlayer ();

            if ( !ownerPlayer )
            {
                return false;
            }

            if ( !ownerPlayer.CanBuild () )
            {
                ownerPlayer.ChatMessage ( "Building is blocked at player position!" );
                return false;
            }

            var baseEntity = BaseNetworkable.serverEntities.Find ( entityID ) as BaseEntity;
          
            if ( baseEntity == null )
            {
                return false;
            }

            if ( !baseEntity.HasSlot ( deployable.slot ) )
            {
                return false;
            }

            if ( baseEntity.GetSlot ( deployable.slot ) != null )
            {
                return false;
            }

            if ( ownerPlayer.Distance ( baseEntity ) > 3f )
            {
                ownerPlayer.ChatMessage ( "Too far away!" );
                return false;
            }

            if ( !ownerPlayer.CanBuild ( baseEntity.WorldSpaceBounds () ) )
            {
                ownerPlayer.ChatMessage ( "Building is blocked at placement position!" );
                return false    ;
            }

            var ownerItem = __instance.GetOwnerItem ();
            var modDeployable = __instance.GetModDeployable ();
            var baseEntity2 = GameManager.server.CreateEntity ( modDeployable.entityPrefab.resourcePath, default, default, true );
        
            if ( baseEntity2 != null )
            {
                baseEntity2.skinID = ownerItem.skin;
                baseEntity2.SetParent ( baseEntity, baseEntity.GetSlotAnchorName ( deployable.slot ), false, false );
                baseEntity2.OwnerID = ownerPlayer.userID;
                baseEntity2.OnDeployed ( baseEntity, ownerPlayer, ownerItem );
                baseEntity2.Spawn ();
                baseEntity.SetSlot ( deployable.slot, baseEntity2 );
                if ( deployable.placeEffect.isValid )
                {
                    Effect.server.Run ( deployable.placeEffect.resourcePath, baseEntity.transform.position, Vector3.up, null, false );
                }
            }

            modDeployable.OnDeployed ( baseEntity2, ownerPlayer );
            Interface.CallHook ( "OnItemDeployed", __instance, baseEntity, baseEntity2 );
            __instance.UseItemAmount ( 1 );
            return false;
        }
    }
}