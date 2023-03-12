/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Subscription
{
	// Token: 0x0200001A RID: 26
	public class DiscordSubscription
	{
		// Token: 0x0600010B RID: 267 RVA: 0x0000A91E File Offset: 0x00008B1E
		public DiscordSubscription(Snowflake channelId, Plugin plugin, Action<DiscordMessage> callback)
		{
			this._channelId = channelId;
			this._plugin = plugin;
			this._callback = callback;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000A940 File Offset: 0x00008B40
		public bool CanRun(BotClient client)
		{
			bool result;
			if (client != null)
			{
				DiscordClient discordClient = DiscordClient.Clients[this._plugin.Name];
				result = (((discordClient != null) ? discordClient.Bot : null) == client);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000A97C File Offset: 0x00008B7C
		public void Invoke(DiscordMessage message)
		{
			Interface.Oxide.NextTick(delegate()
			{
				try
				{
					this._plugin.TrackStart();
					this._callback(message);
					this._plugin.TrackEnd();
				}
				catch (Exception ex)
				{
					ILogger globalLogger = DiscordExtension.GlobalLogger;
					string str = "An exception occured for discord subscription in channel ";
					string str2 = this._channelId.ToString();
					string str3 = " for plugin ";
					Plugin plugin = this._plugin;
					globalLogger.Exception(str + str2 + str3 + ((plugin != null) ? plugin.Name : null), ex);
				}
			});
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000A9B5 File Offset: 0x00008BB5
		internal void OnRemoved()
		{
			this._plugin = null;
		}

		// Token: 0x040000D8 RID: 216
		private Plugin _plugin;

		// Token: 0x040000D9 RID: 217
		private readonly Action<DiscordMessage> _callback;

		// Token: 0x040000DA RID: 218
		private readonly Snowflake _channelId;
	}
}
