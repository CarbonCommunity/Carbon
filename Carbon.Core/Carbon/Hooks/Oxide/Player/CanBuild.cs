///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using ProtoBuf;

namespace Carbon.Extended
{
    [OxideHook ( "CanPickupEntity", typeof ( object ) ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "this", typeof ( Planner ) )]
    [OxideHook.Parameter ( "prefab", typeof ( Construction ) )]
    [OxideHook.Parameter ( "target", typeof ( Construction.Target ) )]
    [OxideHook.Info ( "Called when the player tries to build something." )]
    [OxideHook.Patch ( typeof ( Planner ), "DoBuild" )]
    public class Planner_DoBuild
    {
        public static bool Prefix ( CreateBuilding msg, ref Planner __instance )
        {
            var ownerPlayer = __instance.GetOwnerPlayer ();
            if ( !ownerPlayer )
            {
                return false;
            }
            if ( ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack ( 1f, float.PositiveInfinity ) )
            {
                ownerPlayer.ChatMessage ( "AntiHack!" );
                return false;
            }

            var construction = PrefabAttribute.server.Find<Construction> ( msg.blockID );
            if ( construction == null )
            {
                ownerPlayer.ChatMessage ( "Couldn't find Construction " + msg.blockID );
                return false;
            }
            if ( !__instance.CanAffordToPlace ( construction ) )
            {
                ownerPlayer.ChatMessage ( "Can't afford to place!" );
                return false;
            }
            if ( !ownerPlayer.CanBuild () && !construction.canBypassBuildingPermission )
            {
                ownerPlayer.ChatMessage ( "Building is blocked!" );
                return false;
            }

            var deployable = __instance.GetDeployable ();
            if ( construction.deployable != deployable )
            {
                ownerPlayer.ChatMessage ( "Deployable mismatch!" );
                AntiHack.NoteAdminHack ( ownerPlayer );
                return false;
            }

            var target = default ( Construction.Target );
            if ( msg.entity > 0U )
            {
                var baseEntity = BaseNetworkable.serverEntities.Find ( msg.entity ) as BaseEntity;

                if ( !baseEntity )
                {
                    ownerPlayer.ChatMessage ( "Couldn't find entity " + msg.entity );
                    return false;
                }

                msg.position = baseEntity.transform.TransformPoint ( msg.position );
                msg.normal = baseEntity.transform.TransformDirection ( msg.normal );
                msg.rotation = baseEntity.transform.rotation * msg.rotation;

                if ( msg.socket == 0U )
                {
                    if ( deployable && deployable.setSocketParent && baseEntity.Distance ( msg.position ) > 1f )
                    {
                        ownerPlayer.ChatMessage ( "Parent too far away: " + baseEntity.Distance ( msg.position ) );
                        return false;
                    }
                    if ( baseEntity is Door )
                    {
                        ownerPlayer.ChatMessage ( "Can't deploy on door" );
                        return false;
                    }
                }

                target.entity = baseEntity;

                if ( msg.socket > 0U )
                {
                    string text = StringPool.Get ( msg.socket );
                    if ( text != "" && target.entity != null )
                    {
                        target.socket = __instance.FindSocket ( text, target.entity.prefabID );
                    }
                    else
                    {
                        ownerPlayer.ChatMessage ( "Invalid Socket!" );
                    }
                }
            }
            target.ray = msg.ray;
            target.onTerrain = msg.onterrain;
            target.position = msg.position;
            target.normal = msg.normal;
            target.rotation = msg.rotation;
            target.player = ownerPlayer;
            target.valid = true;
            if ( Interface.CallHook ( "CanBuild", __instance, construction, target ) != null )
            {
                return false;
            }
            __instance.DoBuild ( target, construction );
            return false;
        }
    }
}