///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.AntiCheatServer;
using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnPlayerBanned"), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("connection", typeof(Network.Connection))]
	[OxideHook.Parameter("reason", typeof(string))]
	[OxideHook.Info("Called when a player has been banned from the server.")]
	[OxideHook.Info("Will have reason available if provided.")]
	[OxideHook.Patch(typeof(EACServer), "OnClientActionRequired")]
	public class EACServer_OnClientActionRequired_OnPlayerBanned
	{
		public static bool Prefix(ref OnClientActionRequiredCallbackInfo data)
		{
			var clientHandle = data.ClientHandle;
			var connection = EACServer.GetConnection(clientHandle);

			if (connection == null)
			{
				Carbon.Logger.Error(
					$"[EAC] Status update for invalid client: {clientHandle.ToString()}"
				);
			}
			else
			{
				AntiCheatCommonClientAction clientAction = data.ClientAction;

				if (clientAction == AntiCheatCommonClientAction.RemovePlayer)
				{
					var actionReasonDetailsString = data.ActionReasonDetailsString;

					Carbon.Logger.Log(string.Format("[EAC] Kicking {0} / {1} ({2})", connection.userid, connection.username, actionReasonDetailsString));
					connection.authStatus = "eac";
					Network.Net.sv.Kick(connection, "EAC: " + actionReasonDetailsString, false);
					Interface.CallHook("OnPlayerKicked", connection, actionReasonDetailsString.ToString());

					if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned || data.ActionReasonCode == AntiCheatCommonClientActionReason.TemporaryBanned)
					{
						connection.authStatus = "eacbanned";
						ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Kicking " + connection.username + " (banned by anticheat)");
						Interface.CallHook("OnPlayerBanned", connection, actionReasonDetailsString.ToString());
					}

					var unregisterClientOptions = new UnregisterClientOptions { ClientHandle = clientHandle };
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					EACServer.client2connection.Remove(clientHandle);
					EACServer.connection2client.Remove(connection);
					EACServer.connection2status.Remove(connection);
				}
			}

			return false;
		}
	}

	[OxideHook("OnPlayerKicked"), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Require("OnPlayerBanned")]
	[OxideHook.Parameter("connection", typeof(Network.Connection))]
	[OxideHook.Parameter("reason", typeof(string))]
	[OxideHook.Info("Called after the player is kicked from the server.")]
	[OxideHook.Patch(typeof(EACServer), "OnClientActionRequired")]
	public class EACServer_OnClientActionRequired_OnPlayerKicked
	{
		public static void Prefix(ref OnClientActionRequiredCallbackInfo data)
		{

		}
	}
}
