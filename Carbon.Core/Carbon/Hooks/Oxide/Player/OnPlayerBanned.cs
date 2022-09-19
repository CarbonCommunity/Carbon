using Carbon.Core;
using CompanionServer.Handlers;
using EasyAntiCheat.Server.Hydra;
using Harmony;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;

namespace Carbon.Extended
{
    [Hook ( "OnUserBanned", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "connection", typeof ( Network.Connection ) )]
    [Hook.Parameter ( "reason", typeof ( string ) )]
    [Hook.Info ( "Called when a player has been banned from the server." )]
    [Hook.Info ( "Will have reason available if provided." )]
    [Hook.Patch ( typeof ( EACServer ), "HandleClientUpdate" )]
    public class EACServer_HandleClientUpdate_OnPlayerBanned
    {
        public static void Prefix ( ClientStatusUpdate<EasyAntiCheat.Server.Hydra.Client> clientStatus )
        {
            var client = clientStatus.Client;
            var connection = EACServer.GetConnection ( client );

            if ( !EACServer.ShouldIgnore ( connection ) )
            {
                if ( clientStatus.RequiresKick )
                {
                    var text = clientStatus.Message;

                    if ( string.IsNullOrEmpty ( text ) )
                    {
                        text = clientStatus.Status.ToString ();
                    }

                    if ( clientStatus.IsBanned ( out var dateTime ) )
                    {
                        connection.authStatus = "eacbanned";
                        Interface.CallHook ( "OnPlayerBanned", connection, text );
                    }
                }
            }
        }
    }

    [Hook ( "OnPlayerKicked", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Player )]
    [Hook.Parameter ( "connection", typeof ( Network.Connection ) )]
    [Hook.Parameter ( "reason", typeof ( string ) )]
    [Hook.Info ( "Called after the player is kicked from the server." )]
    [Hook.Patch ( typeof ( EACServer ), "HandleClientUpdate" )]
    public class EACServer_HandleClientUpdate_OnPlayerKicked
    {
        public static void Prefix ( ClientStatusUpdate<EasyAntiCheat.Server.Hydra.Client> clientStatus )
        {
            var client = clientStatus.Client;
            var connection = EACServer.GetConnection ( client );

            if ( !EACServer.ShouldIgnore ( connection ) )
            {
                if ( clientStatus.RequiresKick )
                {
                    var text = clientStatus.Message;

                    if ( string.IsNullOrEmpty ( text ) )
                    {
                        text = clientStatus.Status.ToString ();
                    }

                    connection.authStatus = "eac";
                    Network.Net.sv.Kick ( connection, "EAC: " + text, false );
                    Interface.CallHook ( "OnPlayerKicked", connection, text );
                }
            }
        }
    }
}