using System;
using API.Hooks;
using Network;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class WhitelistModule
{
	[HookAttribute.Patch("IOnNetworkMessage", "IOnNetworkMessage", typeof(ServerMgr), "OnNetworkMessage", new System.Type[] { typeof(Message) })]
	[HookAttribute.Identifier("c05d9e871bf349819d15588fbbab31e3")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class ServerMgr_OnNetworkMessage_c05d9e871bf349819d15588fbbab31e3 : API.Hooks.Patch
	{
		internal static WhitelistModule Whitelist = GetModule<WhitelistModule>();

		public static void Prefix(Message packet)
		{
			if (packet.type == Message.Type.ConsoleCommand)
			{
				try
				{
					if (!packet.connection.connected)
					{
						if (Whitelist._waitingList.Contains(packet.connection))
						{
							Whitelist.OnPacket(packet);
							return;
						}
					}
				}
				catch(Exception ex) { Logger.Error($"Failed IOnNetworkMessage", ex); }
			}
		}
	}
}
