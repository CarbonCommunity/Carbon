///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;
using Oxide.Core;
using ProtoBuf;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnTeamUpdated", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Team )]
    [OxideHook.Parameter ( "currentTeam", typeof ( ulong ) )]
    [OxideHook.Parameter ( "playerTeam", typeof ( PlayerTeam ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Called when player's team is updated." )]
    [OxideHook.Patch ( typeof ( BasePlayer ), "TeamUpdate" )]
    public class BasePlayer_TeamUpdate
    {
        public static bool Prefix ( ref BasePlayer __instance )
        {
            if ( !RelationshipManager.TeamsEnabled () )
            {
                return false;
            }
            if ( __instance.IsConnected && __instance.currentTeam != 0UL )
            {
               var  playerTeam = RelationshipManager.ServerInstance.FindTeam ( __instance.currentTeam );

                if ( playerTeam == null )
                {
                    return false;
                }

                int num = 0;
                int num2 = 0;

                using ( var playerTeam2 = Facepunch.Pool.Get<PlayerTeam> () )
                {
                    playerTeam2.teamLeader = playerTeam.teamLeader;
                    playerTeam2.teamID = playerTeam.teamID;
                    playerTeam2.teamName = playerTeam.teamName;
                    playerTeam2.members = Facepunch.Pool.GetList<PlayerTeam.TeamMember> ();
                    playerTeam2.teamLifetime = playerTeam.teamLifetime;
                    foreach ( ulong playerID in playerTeam.members )
                    {
                        var basePlayer = RelationshipManager.FindByID ( playerID );
                        var teamMember = Facepunch.Pool.Get<PlayerTeam.TeamMember> ();

                        teamMember.displayName = ( ( basePlayer != null ) ? basePlayer.displayName : ( SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName ( playerID ) ?? "DEAD" ) );
                        teamMember.healthFraction = ( ( basePlayer != null && basePlayer.IsAlive () ) ? basePlayer.healthFraction : 0f );
                        teamMember.position = ( ( basePlayer != null ) ? basePlayer.transform.position : Vector3.zero );
                        teamMember.online = ( basePlayer != null && !basePlayer.IsSleeping () );
                        teamMember.wounded = ( basePlayer != null && basePlayer.IsWounded () );

                        if ( ( !__instance.sentInstrumentTeamAchievement || !__instance.sentSummerTeamAchievement ) && basePlayer != null )
                        {
                            if ( basePlayer.GetHeldEntity () && basePlayer.GetHeldEntity ().IsInstrument () )
                            {
                                num++;
                            }
                            if ( basePlayer.isMounted )
                            {
                                if ( basePlayer.GetMounted ().IsInstrument () )
                                {
                                    num++;
                                }
                                if ( basePlayer.GetMounted ().IsSummerDlcVehicle )
                                {
                                    num2++;
                                }
                            }
                            if ( num >= 4 && !__instance.sentInstrumentTeamAchievement )
                            {
                                __instance.GiveAchievement ( "TEAM_INSTRUMENTS" );
                                __instance.sentInstrumentTeamAchievement = true;
                            }
                            if ( num2 >= 4 )
                            {
                                __instance.GiveAchievement ( "SUMMER_INFLATABLE" );
                                __instance.sentSummerTeamAchievement = true;
                            }
                        }
                        teamMember.userID = playerID;
                        playerTeam2.members.Add ( teamMember );
                    }
                    __instance.teamLeaderBuffer = BasePlayer.FindByID ( playerTeam.teamLeader );
                    if ( __instance.teamLeaderBuffer != null )
                    {
                        playerTeam2.mapNote = __instance.teamLeaderBuffer.ServerCurrentMapNote;
                    }
                    if ( Interface.CallHook ( "OnTeamUpdated", __instance.currentTeam, playerTeam2, __instance ) != null )
                    {
                        return false;
                    }
                    __instance.ClientRPCPlayerAndSpectators ( null, __instance, "CLIENT_ReceiveTeamInfo", playerTeam2 );
                    playerTeam2.mapNote = null;
                }
            }
            return false;
        }
    }
}