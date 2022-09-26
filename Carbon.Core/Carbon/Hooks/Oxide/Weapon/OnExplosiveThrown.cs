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
    [OxideHook ( "OnExplosiveThrown" ), OxideHook.Category ( OxideHook.Category.Enum.Weapon )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "entity", typeof ( BaseEntity ) )]
    [OxideHook.Parameter ( "item", typeof ( ThrownWeapon ) )]
    [OxideHook.Info ( "Called when the player throws an explosive item (C4, grenade, ...)." )]
    [OxideHook.Patch ( typeof ( ThrownWeapon ), "DoThrow" )]
    public class ThrownWeapon_DoThrow
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref ThrownWeapon __instance )
        {
            if ( !__instance.HasItemAmount () || __instance.HasAttackCooldown () )
            {
                return false;
            }

            var vector = msg.read.Vector3 ();
            var normalized = msg.read.Vector3 ().normalized;
            var d = Mathf.Clamp01 ( msg.read.Float () );

            if ( msg.player.isMounted || msg.player.HasParent () )
            {
                vector = msg.player.eyes.position;
            }
            else if ( !__instance.ValidateEyePos ( msg.player, vector ) )
            {
                return false;
            }

            var baseEntity = GameManager.server.CreateEntity ( __instance.prefabToThrow.resourcePath, vector, Quaternion.LookRotation ( ( __instance.overrideAngle == Vector3.zero ) ? ( -normalized ) : __instance.overrideAngle ), true );
            if ( baseEntity == null )
            {
                return false;
            }

            baseEntity.creatorEntity = msg.player;
            baseEntity.skinID = __instance.skinID;
            baseEntity.SetVelocity ( __instance.GetInheritedVelocity ( msg.player ) + normalized * __instance.maxThrowVelocity * d + msg.player.estimatedVelocity * 0.5f );
            if ( __instance.tumbleVelocity > 0f )
            {
                baseEntity.SetAngularVelocity ( new Vector3 ( UnityEngine.Random.Range ( -1f, 1f ), UnityEngine.Random.Range ( -1f, 1f ), UnityEngine.Random.Range ( -1f, 1f ) ) * __instance.tumbleVelocity );
            }

            baseEntity.Spawn ();
            __instance.SetUpThrownWeapon ( baseEntity );
            __instance.StartAttackCooldown ( __instance.repeatDelay );
            Interface.CallHook ( "OnExplosiveThrown", msg.player, baseEntity, __instance );
            __instance.UseItemAmount ( 1 );

            var player = msg.player;

            if ( player != null )
            {
                var timedExplosive = baseEntity as TimedExplosive;
                if ( timedExplosive != null )
                {
                    float num = 0f;
                    foreach ( Rust.DamageTypeEntry damageTypeEntry in timedExplosive.damageTypes )
                    {
                        num += damageTypeEntry.amount;
                    }
                    Rust.Ai.Sense.Stimulate ( new Rust.Ai.Sensation
                    {
                        Type = Rust.Ai.SensationType.ThrownWeapon,
                        Position = player.transform.position,
                        Radius = 50f,
                        DamagePotential = num,
                        InitiatorPlayer = player,
                        Initiator = player,
                        UsedEntity = timedExplosive
                    } );
                    return true;
                }
                Rust.Ai.Sense.Stimulate ( new Rust.Ai.Sensation
                {
                    Type = Rust.Ai.SensationType.ThrownWeapon,
                    Position = player.transform.position,
                    Radius = 50f,
                    DamagePotential = 0f,
                    InitiatorPlayer = player,
                    Initiator = player,
                    UsedEntity = __instance
                } );
            }

            return false;
        }
    }
}