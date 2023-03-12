/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Applications;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Entities.Gatway.Events;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.Rest;
using Oxide.Ext.Discord.WebSockets;
using Oxide.Plugins;

namespace Oxide.Ext.Discord
{
	// Token: 0x02000002 RID: 2
	public class BotClient
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public bool Initialized { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
		public DiscordApplication Application { get; internal set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
		public DiscordUser BotUser { get; internal set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
		// (set) Token: 0x06000008 RID: 8 RVA: 0x0000208B File Offset: 0x0000028B
		public RestHandler Rest { get; private set; }

		// Token: 0x06000009 RID: 9 RVA: 0x00002094 File Offset: 0x00000294
		public BotClient(DiscordSettings settings)
		{
			this.Settings = new DiscordSettings
			{
				ApiToken = settings.ApiToken,
				LogLevel = settings.LogLevel,
				Intents = settings.Intents
			};
			this.Logger = new Logger(this.Settings.LogLevel);
			this.Initialized = true;
			this.Rest = new RestHandler(this, this.Logger);
			this._webSocket = new Socket(this, this.Logger);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002148 File Offset: 0x00000348
		public static BotClient GetOrCreate(DiscordClient client)
		{
			BotClient botClient = BotClient.ActiveBots[client.Settings.ApiToken];
			bool flag = botClient == null;
			if (flag)
			{
				DiscordExtension.GlobalLogger.Debug("BotClient.GetOrCreate Creating new BotClient");
				botClient = new BotClient(client.Settings);
				BotClient.ActiveBots[client.Settings.ApiToken] = botClient;
			}
			botClient.AddClient(client);
			ILogger globalLogger = DiscordExtension.GlobalLogger;
			string str = "BotClient.GetOrCreate Adding plugin client ";
			string name = client.Owner.Name;
			string str2 = " to bot ";
			DiscordUser botUser = botClient.BotUser;
			globalLogger.Debug(str + name + str2 + ((botUser != null) ? botUser.GetFullUserName : null));
			return botClient;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021F0 File Offset: 0x000003F0
		public void ConnectWebSocket()
		{
			bool initialized = this.Initialized;
			if (initialized)
			{
				this.Logger.Debug("BotClient.ConnectWebSocket Connecting to websocket");
				this._webSocket.Connect();
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002228 File Offset: 0x00000428
		public void DisconnectWebsocket(bool reconnect = false, bool resume = false)
		{
			bool initialized = this.Initialized;
			if (initialized)
			{
				this._webSocket.Disconnect(reconnect, resume, false);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002254 File Offset: 0x00000454
		internal void ShutdownBot()
		{
			this.Logger.Debug("BotClient.ShutdownBot Shutting down the bot");
			BotClient.ActiveBots.Remove(this.Settings.ApiToken);
			this.Initialized = false;
			Socket webSocket = this._webSocket;
			if (webSocket != null)
			{
				webSocket.Shutdown();
			}
			this._webSocket = null;
			RestHandler rest = this.Rest;
			if (rest != null)
			{
				rest.Shutdown();
			}
			this.Rest = null;
			this.ReadyData = null;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022CC File Offset: 0x000004CC
		public void AddClient(DiscordClient client)
		{
			bool flag = this.Settings.ApiToken != client.Settings.ApiToken;
			if (flag)
			{
				throw new Exception("Failed to add client to bot client as ApiTokens do not match");
			}
			this._clients.RemoveAll((DiscordClient c) => c == client);
			this._clients.Add(client);
			this.Logger.Debug("BotClient.AddClient Add client for plugin " + client.Owner.Title);
			bool flag2 = this._clients.Count == 1;
			if (flag2)
			{
				this.Logger.Debug("BotClient.AddClient Clients.Count == 1 connecting bot");
				this.ConnectWebSocket();
			}
			else
			{
				bool flag3 = client.Settings.LogLevel < this.Settings.LogLevel;
				if (flag3)
				{
					this.UpdateLogLevel(client.Settings.LogLevel);
				}
				GatewayIntents gatewayIntents = this.Settings.Intents | client.Settings.Intents;
				bool flag4 = gatewayIntents != this.Settings.Intents;
				if (flag4)
				{
					this.Logger.Info("New intents have been requested for the bot. Reconnecting with updated intents.");
					this.Settings.Intents = gatewayIntents;
					this.DisconnectWebsocket(true, false);
				}
				bool flag5 = this.ReadyData != null;
				if (flag5)
				{
					this.ReadyData.Guilds = this.Servers.Copy<Snowflake, DiscordGuild>();
					client.CallHook("OnDiscordGatewayReady", new object[]
					{
						this.ReadyData
					});
					foreach (DiscordGuild discordGuild in this.Servers.Values)
					{
						bool isAvailable = discordGuild.IsAvailable;
						if (isAvailable)
						{
							client.CallHook("OnDiscordGuildCreated", new object[]
							{
								discordGuild
							});
						}
						bool hasLoadedAllMembers = discordGuild.HasLoadedAllMembers;
						if (hasLoadedAllMembers)
						{
							client.CallHook("OnDiscordGuildMembersLoaded", new object[]
							{
								discordGuild
							});
						}
					}
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002518 File Offset: 0x00000718
		public void RemoveClient(DiscordClient client)
		{
			this._clients.Remove(client);
			this.Logger.Debug("BotClient.RemoveClient Client Removed");
			bool flag = this._clients.Count == 0;
			if (flag)
			{
				this.ShutdownBot();
				this.Logger.Debug("BotClient.RemoveClient Bot count 0 shutting down bot");
			}
			else
			{
				DiscordLogLevel discordLogLevel = DiscordLogLevel.Off;
				for (int i = 0; i < this._clients.Count; i++)
				{
					DiscordClient discordClient = this._clients[i];
					bool flag2 = discordClient.Settings.LogLevel < discordLogLevel;
					if (flag2)
					{
						discordLogLevel = discordClient.Settings.LogLevel;
					}
				}
				bool flag3 = discordLogLevel > this.Settings.LogLevel;
				if (flag3)
				{
					this.UpdateLogLevel(discordLogLevel);
				}
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000025E0 File Offset: 0x000007E0
		private void UpdateLogLevel(DiscordLogLevel level)
		{
			this.Logger.UpdateLogLevel(level);
			this.Logger.Debug("BotClient.UpdateLogLevel Updating log level from: " + this.Settings.LogLevel.ToString() + " to: " + level.ToString());
			this.Settings.LogLevel = level;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002648 File Offset: 0x00000848
		public void CallHook(string hookName, params object[] args)
		{
			for (int i = 0; i < this._clients.Count; i++)
			{
				DiscordClient discordClient = this._clients[i];
				discordClient.CallHook(hookName, args);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002688 File Offset: 0x00000888
		public void RequestGuildMembers(GuildMembersRequestCommand request)
		{
			bool flag = !this.Initialized;
			if (!flag)
			{
				this._webSocket.Send(GatewayCommandCode.RequestGuildMembers, request);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000026B4 File Offset: 0x000008B4
		public void UpdateVoiceState(UpdateVoiceStatusCommand voiceState)
		{
			bool flag = !this.Initialized;
			if (!flag)
			{
				this._webSocket.Send(GatewayCommandCode.VoiceStateUpdate, voiceState);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026E0 File Offset: 0x000008E0
		public void UpdateStatus(UpdatePresenceCommand presenceUpdate)
		{
			bool flag = !this.Initialized;
			if (!flag)
			{
				this._webSocket.Send(GatewayCommandCode.PresenceUpdate, presenceUpdate);
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000270C File Offset: 0x0000090C
		public DiscordGuild GetGuild(Snowflake? guildId)
		{
			bool flag = guildId != null && guildId.Value.IsValid();
			DiscordGuild result;
			if (flag)
			{
				result = this.Servers[guildId.Value];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002754 File Offset: 0x00000954
		public DiscordChannel GetChannel(Snowflake channelId, Snowflake? guildId)
		{
			DiscordChannel result;
			if (guildId == null)
			{
				result = this.DirectMessagesByChannelId[channelId];
			}
			else
			{
				DiscordGuild guild = this.GetGuild(guildId);
				if (guild == null)
				{
					result = null;
				}
				else
				{
					Hash<Snowflake, DiscordChannel> channels = guild.Channels;
					result = ((channels != null) ? channels[channelId] : null);
				}
			}
			return result;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000279D File Offset: 0x0000099D
		public void AddGuild(DiscordGuild guild)
		{
			this.Servers[guild.Id] = guild;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027B4 File Offset: 0x000009B4
		public void AddGuildOrUpdate(DiscordGuild guild)
		{
			DiscordGuild discordGuild = this.Servers[guild.Id];
			bool flag = discordGuild != null;
			if (flag)
			{
				this.Logger.Verbose("BotClient.AddGuildOrUpdate Updating Existing Guild " + guild.Id.ToString());
				discordGuild.Update(guild);
			}
			else
			{
				this.Logger.Verbose("BotClient.AddGuildOrUpdate Adding new Guild " + guild.Id.ToString());
				this.Servers[guild.Id] = guild;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002851 File Offset: 0x00000A51
		internal void RemoveGuild(Snowflake guildId)
		{
			this.Servers.Remove(guildId);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002864 File Offset: 0x00000A64
		public void AddDirectChannel(DiscordChannel channel)
		{
			bool flag = channel.Type != ChannelType.Dm;
			if (flag)
			{
				this.Logger.Warning("BotClient.AddDirectChannel Tried to add non DM channel");
			}
			else
			{
				this.Logger.Verbose("BotClient.AddDirectChannel Adding New Channel " + channel.Id.ToString());
				this.DirectMessagesByChannelId[channel.Id] = channel;
				foreach (KeyValuePair<Snowflake, DiscordUser> keyValuePair in channel.Recipients)
				{
					bool flag2 = keyValuePair.Value.Bot == null || !keyValuePair.Value.Bot.Value;
					if (flag2)
					{
						this.DirectMessagesByUserId[keyValuePair.Value.Id] = channel;
					}
				}
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002968 File Offset: 0x00000B68
		public void RemoveDirectMessageChannel(Snowflake id)
		{
			DiscordChannel discordChannel = this.DirectMessagesByChannelId[id];
			bool flag = discordChannel != null;
			if (flag)
			{
				this.DirectMessagesByChannelId.Remove(id);
				this.DirectMessagesByUserId.RemoveAll((DiscordChannel c) => c.Id == id);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000029CC File Offset: 0x00000BCC
		internal bool IsPluginRegistered(Plugin plugin)
		{
			for (int i = 0; i < this._clients.Count; i++)
			{
				DiscordClient discordClient = this._clients[i];
				bool flag = discordClient.RegisteredForHooks.Contains(plugin);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000001 RID: 1
		public static readonly Hash<string, BotClient> ActiveBots = new Hash<string, BotClient>();

		// Token: 0x04000002 RID: 2
		public readonly DiscordSettings Settings;

		// Token: 0x04000003 RID: 3
		public readonly Hash<Snowflake, DiscordGuild> Servers = new Hash<Snowflake, DiscordGuild>();

		// Token: 0x04000004 RID: 4
		public readonly Hash<Snowflake, DiscordChannel> DirectMessagesByChannelId = new Hash<Snowflake, DiscordChannel>();

		// Token: 0x04000005 RID: 5
		public readonly Hash<Snowflake, DiscordChannel> DirectMessagesByUserId = new Hash<Snowflake, DiscordChannel>();

		// Token: 0x0400000A RID: 10
		internal readonly ILogger Logger;

		// Token: 0x0400000B RID: 11
		internal GatewayReadyEvent ReadyData;

		// Token: 0x0400000C RID: 12
		private Socket _webSocket;

		// Token: 0x0400000D RID: 13
		private readonly List<DiscordClient> _clients = new List<DiscordClient>();
	}
}
