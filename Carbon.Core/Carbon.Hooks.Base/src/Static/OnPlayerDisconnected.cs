/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using Oxide.Core;

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		[HookAttribute.Patch("OnPlayerDisconnected", typeof(ServerMgr), "OnDisconnected", new System.Type[] { typeof(string), typeof(Network.Connection) })]
		[HookAttribute.Identifier("4d9dcdaf8fbd4923a96eef18a7da7488")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		// Called after the player has disconnected from the server.

		public class Static_ServerMgr_OnDisconnected_4d9dcdaf8fbd4923a96eef18a7da7488
		{
			public static bool Prefix(string strReason, Network.Connection connection, ref ServerMgr __instance)
			{
				__instance.connectionQueue.RemoveConnection(connection);
				ConnectionAuth.OnDisconnect(connection);
				PlatformService.Instance.EndPlayerSession(connection.userid);
				EACServer.OnLeaveGame(connection);

				if (connection.player is BasePlayer player)
				{
					HookCaller.CallStaticHook("OnPlayerDisconnected", player, strReason);
					player.OnDisconnected();
				}

				return false;
			}
		}
	}
}
