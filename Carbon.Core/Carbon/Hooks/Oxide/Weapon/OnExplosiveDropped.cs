///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnExplosiveDropped" ), OxideHook.Category ( OxideHook.Category.Enum.Weapon )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "entity", typeof ( BaseEntity ) )]
    [OxideHook.Parameter ( "item", typeof ( ThrownWeapon ) )]
    [OxideHook.Info ( "Called when the player drops an explosive item (C4, grenade, ...)." )]
    [OxideHook.Patch ( typeof ( ThrownWeapon ), "DoDrop" )]
    public class ThrownWeapon_DoDrop
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref ThrownWeapon __instance )
        {
            if ( !__instance.HasItemAmount () || __instance.HasAttackCooldown () )
            {
                return true;
            }
            Vector3 vector = msg.read.Vector3 ();
            Vector3 normalized = msg.read.Vector3 ().normalized;
            if ( msg.player.isMounted || msg.player.HasParent () )
            {
                vector = msg.player.eyes.position;
            }
            else if ( !__instance.ValidateEyePos ( msg.player, vector ) )
            {
                return true;
            }
            global::BaseEntity baseEntity = global::GameManager.server.CreateEntity ( __instance.prefabToThrow.resourcePath, vector, Quaternion.LookRotation ( Vector3.up ), true );
            if ( baseEntity == null )
            {
                return true;
            }
            RaycastHit hit;
            if ( __instance.canStick && UnityEngine.Physics.SphereCast ( new Ray ( vector, normalized ), 0.05f, out hit, 1.5f, 1236478737 ) )
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;
                global::BaseEntity baseEntity2 = hit.GetEntity ();
                Collider collider = hit.collider;
                if ( baseEntity2 && baseEntity2 is global::StabilityEntity && baseEntity is global::TimedExplosive )
                {
                    baseEntity2 = baseEntity2.ToServer<global::BaseEntity> ();
                    global::TimedExplosive timedExplosive = baseEntity as global::TimedExplosive;
                    timedExplosive.onlyDamageParent = true;
                    timedExplosive.DoStick ( point, normal, baseEntity2, collider );
                }
                else
                {
                    baseEntity.SetVelocity ( normalized );
                }
            }
            else
            {
                baseEntity.SetVelocity ( normalized );
            }
            baseEntity.creatorEntity = msg.player;
            baseEntity.skinID = __instance.skinID;
            baseEntity.Spawn ();
            __instance.SetUpThrownWeapon ( baseEntity );
            __instance.StartAttackCooldown ( __instance.repeatDelay );
            Interface.CallHook ( "OnExplosiveDropped", msg.player, baseEntity, __instance );
            __instance.UseItemAmount ( 1 );

            return false;
        }
    }
}