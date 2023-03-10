/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Subscription
{
	// Token: 0x0200001B RID: 27
	public class DiscordSubscriptions : Library
	{
		// Token: 0x0600010F RID: 271 RVA: 0x0000A9BF File Offset: 0x00008BBF
		public DiscordSubscriptions(ILogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000A9DC File Offset: 0x00008BDC
		public bool HasSubscriptions()
		{
			return this._subscriptions.Count != 0;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000A9FC File Offset: 0x00008BFC
		public void AddChannelSubscription(Plugin plugin, Snowflake channelId, Action<DiscordMessage> message)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			bool flag2 = !channelId.IsValid();
			if (flag2)
			{
				throw new ArgumentException("Value should be valid.", "channelId");
			}
			bool flag3 = message == null;
			if (flag3)
			{
				throw new ArgumentNullException("message");
			}
			this._logger.Debug("DiscordSubscriptions.AddChannelSubscription " + plugin.Name + " added subscription to channel " + channelId.ToString());
			Hash<string, DiscordSubscription> hash = this._subscriptions[channelId];
			bool flag4 = hash == null;
			if (flag4)
			{
				hash = new Hash<string, DiscordSubscription>();
				this._subscriptions[channelId] = hash;
			}
			hash[plugin.Name] = new DiscordSubscription(channelId, plugin, message);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000AAC4 File Offset: 0x00008CC4
		public void RemoveChannelSubscription(Plugin plugin, Snowflake channelId)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			bool flag2 = !channelId.IsValid();
			if (flag2)
			{
				throw new ArgumentException("Value should be valid.", "channelId");
			}
			Hash<string, DiscordSubscription> hash = this._subscriptions[channelId];
			bool flag3 = hash == null;
			if (!flag3)
			{
				hash.Remove(plugin.Name);
				bool flag4 = hash.Count == 0;
				if (flag4)
				{
					this._subscriptions.Remove(channelId);
				}
				this._logger.Debug("DiscordSubscriptions.RemoveChannelSubscription " + plugin.Name + " removed subscription to channel " + channelId.ToString());
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000AB76 File Offset: 0x00008D76
		internal void OnPluginUnloaded(Plugin plugin)
		{
			this.RemovePluginSubscriptions(plugin);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000AB84 File Offset: 0x00008D84
		public void RemovePluginSubscriptions(Plugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			List<Snowflake> list = new List<Snowflake>();
			int num = 0;
			foreach (KeyValuePair<Snowflake, Hash<string, DiscordSubscription>> keyValuePair in this._subscriptions)
			{
				DiscordSubscription discordSubscription = keyValuePair.Value[plugin.Name];
				bool flag2 = discordSubscription != null;
				if (flag2)
				{
					keyValuePair.Value.Remove(plugin.Name);
					discordSubscription.OnRemoved();
					num++;
					bool flag3 = keyValuePair.Value.Count == 0;
					if (flag3)
					{
						list.Add(keyValuePair.Key);
					}
				}
			}
			bool flag4 = list.Count != 0;
			if (flag4)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Snowflake snowflake = list[i];
					this._subscriptions.Remove(snowflake);
				}
			}
			this._logger.Debug("DiscordSubscriptions.RemovePluginSubscriptions Removed " + num.ToString() + " subscriptions for plugin " + plugin.Name);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000ACC0 File Offset: 0x00008EC0
		internal void HandleMessage(DiscordMessage message, DiscordChannel channel, BotClient client)
		{
			this.RunSubs(this._subscriptions[message.ChannelId], message, client);
			bool flag = channel.ParentId != null;
			if (flag)
			{
				this.RunSubs(this._subscriptions[channel.ParentId.Value], message, client);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000AD20 File Offset: 0x00008F20
		private void RunSubs(Hash<string, DiscordSubscription> subs, DiscordMessage message, BotClient client)
		{
			bool flag = subs == null;
			if (!flag)
			{
				foreach (DiscordSubscription discordSubscription in subs.Values)
				{
					bool flag2 = discordSubscription.CanRun(client);
					if (flag2)
					{
						discordSubscription.Invoke(message);
					}
				}
			}
		}

		// Token: 0x040000DB RID: 219
		private readonly Hash<Snowflake, Hash<string, DiscordSubscription>> _subscriptions = new Hash<Snowflake, Hash<string, DiscordSubscription>>();

		// Token: 0x040000DC RID: 220
		private readonly ILogger _logger;
	}
}
