///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;
using Oxide.Core;
using ProtoBuf;

namespace Carbon.Extended
{
    [OxideHook ( "CanPickupEntity", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
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
            var activeGameMode = BaseGameMode.GetActiveGameMode ( true );
            BaseGameMode.CanBuildResult? canBuildResult = ( activeGameMode != null ) ? activeGameMode.CanBuildEntity ( ownerPlayer, construction ) : null;
            if ( canBuildResult != null )
            {
                if ( canBuildResult.Value.Phrase != null )
                {
                    ownerPlayer.ShowToast ( canBuildResult.Value.Result ? GameTip.Styles.Blue_Long : GameTip.Styles.Red_Normal, canBuildResult.Value.Phrase, canBuildResult.Value.Arguments );
                }
                if ( !canBuildResult.Value.Result )
                {
                    return false;
                }
            }
            var target = default ( Construction.Target );
            if ( msg.entity > 0U )
            {
                target.entity = ( BaseNetworkable.serverEntities.Find ( msg.entity ) as BaseEntity );
                if ( target.entity == null )
                {
                    ownerPlayer.ChatMessage ( "Couldn't find entity " + msg.entity );
                    return false;
                }
                msg.position = target.entity.transform.TransformPoint ( msg.position );
                msg.normal = target.entity.transform.TransformDirection ( msg.normal );
                msg.rotation = target.entity.transform.rotation * msg.rotation;
                if ( msg.socket > 0U )
                {
                    string text = StringPool.Get ( msg.socket );
                    if ( text != "" )
                    {
                        target.socket = __instance.FindSocket ( text, target.entity.prefabID );
                    }
                    if ( target.socket == null )
                    {
                        ownerPlayer.ChatMessage ( "Couldn't find socket " + msg.socket );
                        return false;
                    }
                }
                else if ( target.entity is Door )
                {
                    ownerPlayer.ChatMessage ( "Can't deploy on door" );
                    return false;
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

            if ( target.entity != null && deployable != null && deployable.setSocketParent )
            {
                var position = ( target.socket != null ) ? target.GetWorldPosition () : target.position;
                var num = target.entity.Distance ( position );
                if ( num > 1f )
                {
                    ownerPlayer.ChatMessage ( "Parent too far away: " + num );
                    return false;
                }
            }
            __instance.DoBuild ( target, construction );
            return false;
        }
    }
}