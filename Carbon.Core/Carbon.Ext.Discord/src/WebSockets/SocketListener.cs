/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Stages;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Entities.Gatway.Events;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents;
using Oxide.Ext.Discord.Entities.Interactions;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Stickers;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Voice;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.WebSockets.Handlers;
using Oxide.Plugins;
using WebSocketSharp;

namespace Oxide.Ext.Discord.WebSockets
{
	// Token: 0x02000009 RID: 9
	public class SocketListener
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000038A5 File Offset: 0x00001AA5
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000038AD File Offset: 0x00001AAD
		public bool SocketHasConnected { get; internal set; }

		// Token: 0x0600004F RID: 79 RVA: 0x000038B8 File Offset: 0x00001AB8
		public SocketListener(BotClient client, Socket socket, ILogger logger, SocketCommandHandler commands)
		{
			this._client = client;
			this._webSocket = socket;
			this._logger = logger;
			this._commands = commands;
			this._heartbeat = new HeartbeatHandler(this._client, this._webSocket, this, this._logger);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003908 File Offset: 0x00001B08
		public void Shutdown()
		{
			HeartbeatHandler heartbeat = this._heartbeat;
			if (heartbeat != null)
			{
				heartbeat.DestroyHeartbeat();
			}
			this._heartbeat = null;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003924 File Offset: 0x00001B24
		public void SocketOpened(object sender, EventArgs e)
		{
			this._logger.Info("Discord socket opened!");
			this._webSocket.SocketState = SocketState.Connected;
			this._client.CallHook("OnDiscordWebsocketOpened", Array.Empty<object>());
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000395C File Offset: 0x00001B5C
		public void SocketClosed(object sender, CloseEventArgs e)
		{
			WebSocket webSocket = sender as WebSocket;
			bool flag = webSocket != null && !this._webSocket.IsCurrentSocket(webSocket);
			if (flag)
			{
				this._logger.Debug("SocketListener.SocketClosed Socket closed event for non matching socket. Code: " + e.Code.ToString() + ", reason: " + e.Reason);
			}
			else
			{
				bool flag2 = e.Code == 1000 || e.Code == 4199;
				if (flag2)
				{
					this._logger.Debug("SocketListener.SocketClosed Discord WebSocket closed. Code: " + e.Code.ToString() + ", reason: " + e.Reason);
				}
				this._client.CallHook("OnDiscordWebsocketClosed", new object[]
				{
					e.Reason,
					e.Code,
					e.WasClean
				});
				this._webSocket.SocketState = SocketState.Disconnected;
				this._webSocket.DisposeSocket();
				this._commands.OnSocketDisconnected();
				bool flag3 = !this._client.Initialized;
				if (!flag3)
				{
					bool requestedReconnect = this._webSocket.RequestedReconnect;
					if (requestedReconnect)
					{
						this._webSocket.RequestedReconnect = false;
						this._webSocket.Reconnect();
					}
					else
					{
						this.HandleDiscordClosedSocket((int)e.Code, e.Reason);
					}
				}
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003AC8 File Offset: 0x00001CC8
		private void HandleDiscordClosedSocket(int code, string reason)
		{
			bool flag = Enum.IsDefined(typeof(SocketCloseCode), code);
			SocketCloseCode socketCloseCode;
			if (flag)
			{
				socketCloseCode = (SocketCloseCode)code;
			}
			else
			{
				bool flag2 = code >= 4000 && code < 5000;
				if (!flag2)
				{
					this._logger.Warning("Discord WebSocket closed with abnormal close code. Code: " + code.ToString() + ", reason: " + reason);
					this._webSocket.Reconnect();
					return;
				}
				socketCloseCode = SocketCloseCode.UnknownCloseCode;
			}
			bool shouldAttemptResume = false;
			bool flag3 = true;
			SocketCloseCode socketCloseCode2 = socketCloseCode;
			SocketCloseCode socketCloseCode3 = socketCloseCode2;
			switch (socketCloseCode3)
			{
			case SocketCloseCode.UnknownError:
				this._logger.Error("Discord had an unknown error. Reconnecting.");
				break;
			case SocketCloseCode.UnknownOpcode:
				this._logger.Error("Unknown gateway opcode sent: " + reason);
				break;
			case SocketCloseCode.DecodeError:
				this._logger.Error("Invalid gateway payload sent: " + reason);
				break;
			case SocketCloseCode.NotAuthenticated:
				this._logger.Error("Tried to send a payload before identifying: " + reason);
				break;
			case SocketCloseCode.AuthenticationFailed:
				this._logger.Error("The given bot token is invalid. Please enter a valid token. Token: " + this._client.Settings.GetHiddenToken());
				flag3 = false;
				break;
			case SocketCloseCode.AlreadyAuthenticated:
				this._logger.Error("The bot has already authenticated. Please don't identify more than once.: " + reason);
				break;
			case (SocketCloseCode)4006:
				break;
			case SocketCloseCode.InvalidSequence:
				this._logger.Error("Invalid resume sequence. Doing full reconnect.: " + reason);
				break;
			case SocketCloseCode.RateLimited:
				this._logger.Error("You're being rate limited. Please slow down how quickly you're sending requests: " + reason);
				shouldAttemptResume = true;
				break;
			case SocketCloseCode.SessionTimedOut:
				this._logger.Error("Session has timed out. Starting a new one: " + reason);
				break;
			case SocketCloseCode.InvalidShard:
				this._logger.Error("Invalid shared has been specified: " + reason);
				flag3 = false;
				break;
			case SocketCloseCode.ShardingRequired:
				this._logger.Error("Bot is in too many guilds. You must shard your bot: " + reason);
				flag3 = false;
				break;
			case SocketCloseCode.InvalidApiVersion:
				this._logger.Error("Gateway is using invalid API version. Please contact Discord Extension Devs immediately!");
				flag3 = false;
				break;
			case SocketCloseCode.InvalidIntents:
				this._logger.Error("Invalid intent(s) specified for the gateway. Please check that you're using valid intents in the connect.");
				flag3 = false;
				break;
			case SocketCloseCode.DisallowedIntent:
				this._logger.Error("The plugin is asking for an intent you have not granted your bot. Please complete step 5 @ https://umod.org/extensions/discord#getting-your-api-key");
				flag3 = false;
				break;
			default:
				if (socketCloseCode3 == SocketCloseCode.UnknownCloseCode)
				{
					this._logger.Error(string.Concat(new string[]
					{
						"Discord has closed the gateway with a code we do not recognize. Code: ",
						code.ToString(),
						". Reason: ",
						reason,
						" Please Contact Discord Extension Authors."
					}));
				}
				break;
			}
			bool flag4 = flag3;
			if (flag4)
			{
				this._webSocket.ShouldAttemptResume = shouldAttemptResume;
				this._webSocket.Reconnect();
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003D98 File Offset: 0x00001F98
		public void SocketErrored(object sender, ErrorEventArgs e)
		{
			WebSocket webSocket = sender as WebSocket;
			bool flag = webSocket != null && !this._webSocket.IsCurrentSocket(webSocket);
			if (!flag)
			{
				this._client.CallHook("OnDiscordWebsocketErrored", new object[]
				{
					e.Exception,
					e.Message
				});
				this._logger.Exception("An error has occured in the websocket. Attempting to reconnect to discord.", e.Exception);
				this._webSocket.Disconnect(true, false, false);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003E18 File Offset: 0x00002018
		public void SocketMessage(object sender, MessageEventArgs e)
		{
			EventPayload eventPayload = JsonConvert.DeserializeObject<EventPayload>(e.Data);
			Carbon.Logger.Log($"{e.Data}");
			bool flag = eventPayload.Sequence != null;
			if (flag)
			{
				this._sequence = eventPayload.Sequence.Value;
			}
			bool flag2 = this._logger.IsLogging(DiscordLogLevel.Verbose);
			if (flag2)
			{
				this._logger.Verbose("Received socket message, OpCode: " + eventPayload.OpCode.ToString() + "\nContent:\n" + e.Data);
			}
			else
			{
				this._logger.Debug("Received socket message, OpCode: " + eventPayload.OpCode.ToString());
			}
			try
			{
				GatewayEventCode opCode = eventPayload.OpCode;
				GatewayEventCode gatewayEventCode = opCode;
				if (gatewayEventCode != GatewayEventCode.Dispatch)
				{
					if (gatewayEventCode != GatewayEventCode.Heartbeat)
					{
						switch (gatewayEventCode)
						{
						case GatewayEventCode.Reconnect:
							this.HandleReconnect(eventPayload);
							goto IL_12C;
						case GatewayEventCode.InvalidSession:
							this.HandleInvalidSession(eventPayload);
							goto IL_12C;
						case GatewayEventCode.Hello:
							this.HandleHello(eventPayload);
							goto IL_12C;
						case GatewayEventCode.HeartbeatAcknowledge:
							this.HandleHeartbeatAcknowledge(eventPayload);
							goto IL_12C;
						}
						this.UnhandledOpCode(eventPayload);
					}
					else
					{
						this.HandleHeartbeat(eventPayload);
					}
				}
				else
				{
					this.HandleDispatch(eventPayload);
				}
				IL_12C:;
			}
			catch (Exception ex)
			{
				this._logger.Exception("SocketListener.SocketMessage Exception Occured. Please give error message below to Discord Extension Authors:\n", ex);
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003F80 File Offset: 0x00002180
		private void HandleDispatch(EventPayload payload)
		{
			this._logger.Debug(string.Format("Received OpCode: Dispatch, event: {0}", payload.EventName));
			switch (payload.EventCode)
			{
			case DispatchCode.Ready:
				this.HandleDispatchReady(payload);
				return;
			case DispatchCode.Resumed:
				this.HandleDispatchResumed(payload);
				return;
			case DispatchCode.ChannelCreated:
				this.HandleDispatchChannelCreate(payload);
				return;
			case DispatchCode.ChannelUpdated:
				this.HandleDispatchChannelUpdate(payload);
				return;
			case DispatchCode.ChannelDeleted:
				this.HandleDispatchChannelDelete(payload);
				return;
			case DispatchCode.ChannelPinsUpdate:
				this.HandleDispatchChannelPinUpdate(payload);
				return;
			case DispatchCode.GuildCreated:
				this.HandleDispatchGuildCreate(payload);
				return;
			case DispatchCode.GuildUpdated:
				this.HandleDispatchGuildUpdate(payload);
				return;
			case DispatchCode.GuildDeleted:
				this.HandleDispatchGuildDelete(payload);
				return;
			case DispatchCode.GuildBanAdded:
				this.HandleDispatchGuildBanAdd(payload);
				return;
			case DispatchCode.GuildBanRemoved:
				this.HandleDispatchGuildBanRemove(payload);
				return;
			case DispatchCode.GuildEmojisUpdated:
				this.HandleDispatchGuildEmojisUpdate(payload);
				return;
			case DispatchCode.GuildIntegrationsUpdated:
				this.HandleDispatchGuildIntegrationsUpdate(payload);
				return;
			case DispatchCode.GuildMemberAdded:
				this.HandleDispatchGuildMemberAdd(payload);
				return;
			case DispatchCode.GuildMemberRemoved:
				this.HandleDispatchGuildMemberRemove(payload);
				return;
			case DispatchCode.GuildMemberUpdated:
				this.HandleDispatchGuildMemberUpdate(payload);
				return;
			case DispatchCode.GuildMembersChunk:
				this.HandleDispatchGuildMembersChunk(payload);
				return;
			case DispatchCode.GuildRoleCreated:
				this.HandleDispatchGuildRoleCreate(payload);
				return;
			case DispatchCode.GuildRoleUpdated:
				this.HandleDispatchGuildRoleUpdate(payload);
				return;
			case DispatchCode.GuildRoleDeleted:
				this.HandleDispatchGuildRoleDelete(payload);
				return;
			case DispatchCode.GuildScheduledEventCreate:
				this.HandleDispatchGuildScheduledEventCreate(payload);
				return;
			case DispatchCode.GuildScheduledEventUpdate:
				this.HandleDispatchGuildScheduledEventUpdate(payload);
				return;
			case DispatchCode.GuildScheduledEventDelete:
				this.HandleDispatchGuildScheduledEventDelete(payload);
				return;
			case DispatchCode.GuildScheduledEventUserAdd:
				this.HandleDispatchGuildScheduledEventUserAdd(payload);
				return;
			case DispatchCode.GuildScheduledEventUserRemove:
				this.HandleDispatchGuildScheduledEventUserRemove(payload);
				return;
			case DispatchCode.IntegrationCreated:
				this.HandleDispatchIntegrationCreate(payload);
				return;
			case DispatchCode.IntegrationUpdated:
				this.HandleDispatchIntegrationUpdate(payload);
				return;
			case DispatchCode.IntegrationDeleted:
				this.HandleDispatchIntegrationDelete(payload);
				return;
			case DispatchCode.MessageCreated:
				this.HandleDispatchMessageCreate(payload);
				return;
			case DispatchCode.MessageUpdated:
				this.HandleDispatchMessageUpdate(payload);
				return;
			case DispatchCode.MessageDeleted:
				this.HandleDispatchMessageDelete(payload);
				return;
			case DispatchCode.MessageBulkDeleted:
				this.HandleDispatchMessageDeleteBulk(payload);
				return;
			case DispatchCode.MessageReactionAdded:
				this.HandleDispatchMessageReactionAdd(payload);
				return;
			case DispatchCode.MessageReactionRemoved:
				this.HandleDispatchMessageReactionRemove(payload);
				return;
			case DispatchCode.MessageReactionAllRemoved:
				this.HandleDispatchMessageReactionRemoveAll(payload);
				return;
			case DispatchCode.MessageReactionEmojiRemoved:
				this.HandleDispatchMessageReactionRemoveEmoji(payload);
				return;
			case DispatchCode.PresenceUpdated:
				this.HandleDispatchPresenceUpdate(payload);
				return;
			case DispatchCode.PresenceReplace:
				return;
			case DispatchCode.TypingStarted:
				this.HandleDispatchTypingStart(payload);
				return;
			case DispatchCode.UserUpdated:
				this.HandleDispatchUserUpdate(payload);
				return;
			case DispatchCode.VoiceStateUpdated:
				this.HandleDispatchVoiceStateUpdate(payload);
				return;
			case DispatchCode.VoiceServerUpdated:
				this.HandleDispatchVoiceServerUpdate(payload);
				return;
			case DispatchCode.WebhooksUpdated:
				this.HandleDispatchWebhooksUpdate(payload);
				return;
			case DispatchCode.InviteCreated:
				this.HandleDispatchInviteCreate(payload);
				return;
			case DispatchCode.InviteDeleted:
				this.HandleDispatchInviteDelete(payload);
				return;
			case DispatchCode.InteractionCreated:
				this.HandleDispatchInteractionCreate(payload);
				return;
			case DispatchCode.GuildJoinRequestDeleted:
				this.HandleGuildJoinRequestDelete(payload);
				return;
			case DispatchCode.ThreadCreated:
				this.HandleDispatchThreadCreated(payload);
				return;
			case DispatchCode.ThreadUpdated:
				this.HandleDispatchThreadUpdated(payload);
				return;
			case DispatchCode.ThreadDeleted:
				this.HandleDispatchThreadDeleted(payload);
				return;
			case DispatchCode.ThreadListSync:
				this.HandleDispatchThreadListSync(payload);
				return;
			case DispatchCode.ThreadMemberUpdated:
				this.HandleDispatchThreadMemberUpdated(payload);
				return;
			case DispatchCode.ThreadMembersUpdated:
				this.HandleDispatchThreadMembersUpdated(payload);
				return;
			case DispatchCode.StageInstanceCreated:
				this.HandleDispatchStageInstanceCreated(payload);
				return;
			case DispatchCode.StageInstanceUpdated:
				this.HandleDispatchStageInstanceUpdated(payload);
				return;
			case DispatchCode.StageInstanceDeleted:
				this.HandleDispatchStageInstanceDeleted(payload);
				return;
			}
			this.HandleDispatchUnhandledEvent(payload);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000435C File Offset: 0x0000255C
		private void HandleDispatchReady(EventPayload payload)
		{
			GatewayReadyEvent gatewayReadyEvent = payload.EventData.ToObject<GatewayReadyEvent>();
			foreach (DiscordGuild guild in gatewayReadyEvent.Guilds.Values)
			{
				this._client.AddGuildOrUpdate(guild);
			}
			this._sessionId = gatewayReadyEvent.SessionId;
			this._client.Application = gatewayReadyEvent.Application;
			this._client.BotUser = gatewayReadyEvent.User;
			this._webSocket.ResetRetries();
			this._logger.Info("Your bot was found in " + gatewayReadyEvent.Guilds.Count.ToString() + " Guilds!");
			bool flag = this._client.ReadyData == null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGatewayReady", new object[]
				{
					gatewayReadyEvent
				});
			}
			this._client.ReadyData = gatewayReadyEvent;
			this.SocketHasConnected = true;
			this._commands.OnSocketConnected();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004480 File Offset: 0x00002680
		private void HandleDispatchResumed(EventPayload payload)
		{
			GatewayResumedEvent gatewayResumedEvent = payload.EventData.ToObject<GatewayResumedEvent>();
			this._logger.Info("Session resumed successfully!");
			this._client.CallHook("OnDiscordGatewayResumed", new object[]
			{
				gatewayResumedEvent
			});
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000044C8 File Offset: 0x000026C8
		private void HandleDispatchChannelCreate(EventPayload payload)
		{
			DiscordChannel discordChannel = payload.EventData.ToObject<DiscordChannel>();
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchChannelCreate: ID: ",
				discordChannel.Id.ToString(),
				" Type: ",
				discordChannel.Type.ToString(),
				". Guild ID: ",
				discordChannel.GuildId.ToString()
			}));
			bool flag = discordChannel.Type == ChannelType.Dm || discordChannel.Type == ChannelType.GroupDm;
			if (flag)
			{
				this._client.AddDirectChannel(discordChannel);
				this._client.CallHook("OnDiscordDirectChannelCreated", new object[]
				{
					discordChannel
				});
			}
			else
			{
				DiscordGuild guild = this._client.GetGuild(discordChannel.GuildId);
				bool flag2 = guild != null && guild.IsAvailable;
				if (flag2)
				{
					guild.Channels[discordChannel.Id] = discordChannel;
					this._client.CallHook("OnDiscordGuildChannelCreated", new object[]
					{
						discordChannel,
						guild
					});
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000045FC File Offset: 0x000027FC
		private void HandleDispatchChannelUpdate(EventPayload payload)
		{
			DiscordChannel discordChannel = payload.EventData.ToObject<DiscordChannel>();
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchChannelUpdate ID: ",
				discordChannel.Id.ToString(),
				" Type: ",
				discordChannel.Type.ToString(),
				" Guild ID: ",
				discordChannel.GuildId.ToString()
			}));
			bool flag = discordChannel.Type == ChannelType.Dm || discordChannel.Type == ChannelType.GroupDm;
			if (flag)
			{
				DiscordChannel channel = this._client.GetChannel(discordChannel.Id, null);
				bool flag2 = channel == null;
				if (flag2)
				{
					this._client.AddDirectChannel(discordChannel);
					BotClient client = this._client;
					string hookName = "OnDiscordDirectChannelUpdated";
					object[] array = new object[2];
					array[0] = discordChannel;
					client.CallHook(hookName, array);
				}
				else
				{
					DiscordChannel discordChannel2 = channel.Update(discordChannel);
					this._client.CallHook("OnDiscordDirectChannelUpdated", new object[]
					{
						channel,
						discordChannel2
					});
				}
			}
			else
			{
				DiscordGuild guild = this._client.GetGuild(discordChannel.GuildId);
				bool flag3 = guild != null && guild.IsAvailable;
				if (flag3)
				{
					DiscordChannel discordChannel3 = guild.Channels[discordChannel.Id];
					bool flag4 = discordChannel3 != null;
					if (flag4)
					{
						DiscordChannel discordChannel4 = discordChannel3.Update(discordChannel);
						this._client.CallHook("OnDiscordGuildChannelUpdated", new object[]
						{
							discordChannel3,
							discordChannel4,
							guild
						});
					}
					else
					{
						this._logger.Warning(string.Concat(new string[]
						{
							"SocketListener.HandleDispatchChannelUpdate Tried to update channel that doesn't exist: Guild: ",
							guild.Name,
							"(",
							guild.Id.ToString(),
							") Channel: ",
							discordChannel.Name,
							"(",
							discordChannel.Id.ToString(),
							")"
						}));
					}
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004838 File Offset: 0x00002A38
		private void HandleDispatchChannelDelete(EventPayload payload)
		{
			DiscordChannel discordChannel = payload.EventData.ToObject<DiscordChannel>();
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchChannelDelete ID: ",
				discordChannel.Id.ToString(),
				" Type: ",
				discordChannel.Type.ToString(),
				" Guild ID: ",
				discordChannel.GuildId.ToString()
			}));
			DiscordGuild guild = this._client.GetGuild(discordChannel.GuildId);
			bool flag = discordChannel.Type == ChannelType.Dm || discordChannel.Type == ChannelType.GroupDm;
			if (flag)
			{
				this._client.RemoveDirectMessageChannel(discordChannel.Id);
				this._client.CallHook("OnDiscordDirectChannelDeleted", new object[]
				{
					discordChannel
				});
			}
			else
			{
				guild.Channels.Remove(discordChannel.Id);
				this._client.CallHook("OnDiscordGuildChannelDeleted", new object[]
				{
					discordChannel,
					guild
				});
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004958 File Offset: 0x00002B58
		private void HandleDispatchChannelPinUpdate(EventPayload payload)
		{
			ChannelPinsUpdatedEvent channelPinsUpdatedEvent = payload.EventData.ToObject<ChannelPinsUpdatedEvent>();
			this._logger.Verbose("SocketListener.HandleDispatchChannelPinUpdate Channel ID: " + channelPinsUpdatedEvent.GuildId.ToString() + " Guild ID: " + channelPinsUpdatedEvent.GuildId.ToString());
			DiscordGuild guild = this._client.GetGuild(channelPinsUpdatedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(channelPinsUpdatedEvent.ChannelId, channelPinsUpdatedEvent.GuildId);
			bool flag = channelPinsUpdatedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildChannelPinsUpdated", new object[]
				{
					channelPinsUpdatedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectChannelPinsUpdated", new object[]
				{
					channelPinsUpdatedEvent,
					channel
				});
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004A3C File Offset: 0x00002C3C
		private void HandleDispatchGuildCreate(EventPayload payload)
		{
			DiscordGuild discordGuild = payload.EventData.ToObject<DiscordGuild>();
			this._logger.Verbose("SocketListener.HandleDispatchGuildCreate Guild ID: " + discordGuild.Id.ToString() + " Name: " + discordGuild.Name);
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(discordGuild.Id));
			bool flag = guild == null || (!guild.IsAvailable && discordGuild.IsAvailable);
			if (flag)
			{
				this._client.AddGuildOrUpdate(discordGuild);
				guild = this._client.GetGuild(new Snowflake?(discordGuild.Id));
				this._client.CallHook("OnDiscordGuildCreated", new object[]
				{
					guild
				});
				guild.HasLoadedAllMembers = false;
			}
			bool flag2 = !guild.HasLoadedAllMembers && (this._client.Settings.Intents & GatewayIntents.GuildMembers) > GatewayIntents.None;
			if (flag2)
			{
				this._logger.Verbose("SocketListener.HandleDispatchGuildCreate Guild is now requesting all guild members.");
				this._client.RequestGuildMembers(new GuildMembersRequestCommand
				{
					Nonce = "DiscordExtension",
					GuildId = discordGuild.Id
				});
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004B6C File Offset: 0x00002D6C
		private void HandleDispatchGuildUpdate(EventPayload payload)
		{
			DiscordGuild discordGuild = payload.EventData.ToObject<DiscordGuild>();
			this._logger.Verbose("SocketListener.HandleDispatchGuildUpdate Guild ID: " + discordGuild.Id.ToString());
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(discordGuild.Id));
			DiscordGuild discordGuild2 = (guild != null) ? guild.Update(discordGuild) : null;
			this._client.CallHook("OnDiscordGuildUpdated", new object[]
			{
				discordGuild,
				discordGuild2
			});
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004BF4 File Offset: 0x00002DF4
		private void HandleDispatchGuildDelete(EventPayload payload)
		{
			DiscordGuild discordGuild = payload.EventData.ToObject<DiscordGuild>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(discordGuild.Id));
			bool flag = !discordGuild.IsAvailable;
			if (flag)
			{
				this._logger.Verbose("SocketListener.HandleDispatchGuildDelete There is an outage with the guild. Guild ID: " + discordGuild.Id.ToString());
				bool flag2 = guild != null;
				if (flag2)
				{
					guild.Unavailable = discordGuild.Unavailable;
				}
				this._client.CallHook("OnDiscordGuildUnavailable", new object[]
				{
					guild ?? discordGuild
				});
			}
			else
			{
				this._logger.Verbose("SocketListener.HandleDispatchGuildDelete Guild deleted or user removed from guild. Guild ID: " + discordGuild.Id.ToString() + " Name: " + (((guild != null) ? guild.Name : null) ?? discordGuild.Name));
				this._client.RemoveGuild(discordGuild.Id);
				this._client.CallHook("OnDiscordGuildDeleted", new object[]
				{
					guild ?? discordGuild
				});
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004D14 File Offset: 0x00002F14
		private void HandleDispatchGuildBanAdd(EventPayload payload)
		{
			GuildMemberBannedEvent guildMemberBannedEvent = payload.EventData.ToObject<GuildMemberBannedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildMemberBannedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildBanAdd User was banned from the guild. Guild ID: ",
				guildMemberBannedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" User ID: ",
				guildMemberBannedEvent.User.Id.ToString(),
				" User Name: ",
				guildMemberBannedEvent.User.GetFullUserName
			}));
			this._client.CallHook("OnDiscordGuildMemberBanned", new object[]
			{
				guildMemberBannedEvent,
				guild
			});
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004DEC File Offset: 0x00002FEC
		private void HandleDispatchGuildBanRemove(EventPayload payload)
		{
			GuildMemberBannedEvent guildMemberBannedEvent = payload.EventData.ToObject<GuildMemberBannedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildMemberBannedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildBanRemove User was unbanned from the guild. Guild ID: ",
				guildMemberBannedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" User ID: ",
				guildMemberBannedEvent.User.Id.ToString(),
				" User Name: ",
				guildMemberBannedEvent.User.GetFullUserName
			}));
			this._client.CallHook("OnDiscordGuildMemberUnbanned", new object[]
			{
				guildMemberBannedEvent.User,
				guild
			});
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004EC8 File Offset: 0x000030C8
		private void HandleDispatchGuildEmojisUpdate(EventPayload payload)
		{
			GuildEmojisUpdatedEvent emojis = payload.EventData.ToObject<GuildEmojisUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(emojis.GuildId));
			this._logger.Verbose("SocketListener.HandleDispatchGuildEmojisUpdate Guild ID: " + emojis.GuildId.ToString() + " Guild Name: " + ((guild != null) ? guild.Name : null));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				Hash<Snowflake, DiscordEmoji> hash = guild.Emojis.Copy<Snowflake, DiscordEmoji>();
				List<Snowflake> list = new List<Snowflake>();
				foreach (Snowflake snowflake in guild.Emojis.Keys)
				{
					bool flag2 = !emojis.Emojis.ContainsKey(snowflake);
					if (flag2)
					{
						list.Add(snowflake);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					Snowflake snowflake2 = list[i];
					guild.Emojis.Remove(snowflake2);
				}
				guild.Emojis.RemoveAll((DiscordEmoji e) => e.EmojiId != null && !emojis.Emojis.ContainsKey(e.EmojiId.Value));
				foreach (DiscordEmoji discordEmoji in emojis.Emojis.Values)
				{
					DiscordEmoji discordEmoji2 = guild.Emojis[discordEmoji.Id];
					bool flag3 = discordEmoji2 != null;
					if (flag3)
					{
						discordEmoji2.Update(discordEmoji);
					}
					else
					{
						guild.Emojis[discordEmoji.Id] = discordEmoji;
					}
				}
				this._client.CallHook("OnDiscordGuildEmojisUpdated", new object[]
				{
					emojis,
					hash,
					guild
				});
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000050E8 File Offset: 0x000032E8
		private void HandleDispatchGuildStickersUpdate(EventPayload payload)
		{
			GuildStickersUpdatedEvent stickers = payload.EventData.ToObject<GuildStickersUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(stickers.GuildId));
			this._logger.Verbose("SocketListener.HandleDispatchGuildEmojisUpdate Guild ID: " + stickers.GuildId.ToString() + " Guild Name: " + ((guild != null) ? guild.Name : null));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				Hash<Snowflake, DiscordSticker> hash = guild.Stickers.Copy<Snowflake, DiscordSticker>();
				List<Snowflake> list = new List<Snowflake>();
				foreach (Snowflake snowflake in guild.Stickers.Keys)
				{
					bool flag2 = !stickers.Stickers.ContainsKey(snowflake);
					if (flag2)
					{
						list.Add(snowflake);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					Snowflake snowflake2 = list[i];
					guild.Emojis.Remove(snowflake2);
				}
				guild.Emojis.RemoveAll((DiscordEmoji e) => e.EmojiId != null && !stickers.Stickers.ContainsKey(e.EmojiId.Value));
				foreach (DiscordSticker discordSticker in stickers.Stickers.Values)
				{
					DiscordSticker discordSticker2 = guild.Stickers[discordSticker.Id];
					bool flag3 = discordSticker2 != null;
					if (flag3)
					{
						discordSticker2.Update(discordSticker);
					}
					else
					{
						guild.Stickers[discordSticker.Id] = discordSticker;
					}
				}
				this._client.CallHook("OnDiscordGuildStickersUpdated", new object[]
				{
					stickers,
					hash,
					guild
				});
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005308 File Offset: 0x00003508
		private void HandleDispatchGuildIntegrationsUpdate(EventPayload payload)
		{
			GuildIntegrationsUpdatedEvent guildIntegrationsUpdatedEvent = payload.EventData.ToObject<GuildIntegrationsUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildIntegrationsUpdatedEvent.GuildId));
			this._logger.Verbose("SocketListener.HandleDispatchGuildIntegrationsUpdate Guild ID: " + guildIntegrationsUpdatedEvent.GuildId.ToString() + " Guild Name: " + ((guild != null) ? guild.Name : null));
			this._client.CallHook("OnDiscordGuildIntegrationsUpdated", new object[]
			{
				guildIntegrationsUpdatedEvent,
				guild
			});
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005394 File Offset: 0x00003594
		private void HandleDispatchGuildMemberAdd(EventPayload payload)
		{
			GuildMemberAddedEvent guildMemberAddedEvent = payload.EventData.ToObject<GuildMemberAddedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildMemberAddedEvent.GuildId));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				guild.Members[guildMemberAddedEvent.User.Id] = guildMemberAddedEvent;
				this._logger.Verbose(string.Concat(new string[]
				{
					"SocketListener.HandleDispatchGuildMemberAdd Guild ID: ",
					guildMemberAddedEvent.GuildId.ToString(),
					" Guild Name: ",
					guild.Name,
					" User ID: ",
					guildMemberAddedEvent.User.Id.ToString(),
					" User Name: ",
					guildMemberAddedEvent.User.GetFullUserName
				}));
				this._client.CallHook("OnDiscordGuildMemberAdded", new object[]
				{
					guildMemberAddedEvent,
					guild
				});
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005494 File Offset: 0x00003694
		private void HandleDispatchGuildMemberRemove(EventPayload payload)
		{
			GuildMemberRemovedEvent guildMemberRemovedEvent = payload.EventData.ToObject<GuildMemberRemovedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildMemberRemovedEvent.GuildId));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				GuildMember guildMember = guild.Members[guildMemberRemovedEvent.User.Id];
				bool flag2 = guildMember != null;
				if (flag2)
				{
					guild.Members.Remove(guildMemberRemovedEvent.User.Id);
					this._client.CallHook("OnDiscordGuildMemberRemoved", new object[]
					{
						guildMember,
						guild
					});
				}
				this._logger.Verbose(string.Concat(new string[]
				{
					"SocketListener.HandleDispatchGuildMemberRemove Guild ID: ",
					guildMemberRemovedEvent.GuildId.ToString(),
					" Guild Name: ",
					guild.Name,
					" User ID: ",
					(guildMember != null) ? guildMember.User.Id.ToString() : null,
					" User Name: ",
					(guildMember != null) ? guildMember.User.GetFullUserName : null
				}));
				this._client.CallHook("OnDiscordGuildMemberRemoved", new object[]
				{
					guildMemberRemovedEvent,
					guild
				});
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000055E4 File Offset: 0x000037E4
		private void HandleDispatchGuildMemberUpdate(EventPayload payload)
		{
			GuildMemberUpdatedEvent guildMemberUpdatedEvent = payload.EventData.ToObject<GuildMemberUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildMemberUpdatedEvent.GuildId));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				GuildMember guildMember = guild.Members[guildMemberUpdatedEvent.User.Id];
				bool flag2 = guildMember != null;
				if (flag2)
				{
					this._logger.Verbose("SocketListener.HandleDispatchGuildMemberUpdate GUILD_MEMBER_UPDATE: Guild ID: " + guildMemberUpdatedEvent.GuildId.ToString() + " User ID: " + guildMemberUpdatedEvent.User.Id.ToString());
					GuildMember guildMember2 = guildMember.Update(guildMemberUpdatedEvent);
					this._client.CallHook("OnDiscordGuildMemberUpdated", new object[]
					{
						guildMemberUpdatedEvent,
						guildMember2,
						guild
					});
				}
				else
				{
					guild.Members[guildMemberUpdatedEvent.User.Id] = guildMemberUpdatedEvent;
					this._logger.Warning(string.Concat(new string[]
					{
						"SocketListener.HandleDispatchGuildMemberUpdate Tried to update member which does not exist in guild. Guild: ",
						guild.Name,
						"(",
						guild.Id.ToString(),
						") User: ",
						guildMemberUpdatedEvent.User.Username,
						"(",
						guildMemberUpdatedEvent.User.Id.ToString(),
						")"
					}));
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00005770 File Offset: 0x00003970
		private void HandleDispatchGuildMembersChunk(EventPayload payload)
		{
			GuildMembersChunkEvent guildMembersChunkEvent = payload.EventData.ToObject<GuildMembersChunkEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildMembersChunkEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildMembersChunk: Guild ID: ",
				guildMembersChunkEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Nonce: ",
				guildMembersChunkEvent.Nonce
			}));
			bool flag = guildMembersChunkEvent.Nonce == "DiscordExtension";
			if (flag)
			{
				bool flag2 = guild != null && guild.IsAvailable;
				if (flag2)
				{
					for (int i = 0; i < guildMembersChunkEvent.Members.Count; i++)
					{
						GuildMember guildMember = guildMembersChunkEvent.Members[i];
						bool flag3 = !guild.Members.ContainsKey(guildMember.User.Id);
						if (flag3)
						{
							guild.Members[guildMember.User.Id] = guildMember;
						}
					}
					bool flag4 = guildMembersChunkEvent.ChunkIndex + 1 == guildMembersChunkEvent.ChunkCount;
					if (flag4)
					{
						guild.HasLoadedAllMembers = true;
						this._client.CallHook("OnDiscordGuildMembersLoaded", new object[]
						{
							guild
						});
					}
				}
			}
			else
			{
				this._client.CallHook("OnDiscordGuildMembersChunk", new object[]
				{
					guildMembersChunkEvent,
					guild
				});
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000058F8 File Offset: 0x00003AF8
		private void HandleDispatchGuildRoleCreate(EventPayload payload)
		{
			GuildRoleCreatedEvent guildRoleCreatedEvent = payload.EventData.ToObject<GuildRoleCreatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildRoleCreatedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildRoleCreate Guild ID: ",
				guildRoleCreatedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Role ID: ",
				guildRoleCreatedEvent.Role.Id.ToString(),
				" Role Name: ",
				guildRoleCreatedEvent.Role.Name
			}));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				guild.Roles[guildRoleCreatedEvent.Role.Id] = guildRoleCreatedEvent.Role;
				this._client.CallHook("OnDiscordGuildRoleCreated", new object[]
				{
					guildRoleCreatedEvent.Role,
					guild
				});
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005A04 File Offset: 0x00003C04
		private void HandleDispatchGuildRoleUpdate(EventPayload payload)
		{
			GuildRoleUpdatedEvent guildRoleUpdatedEvent = payload.EventData.ToObject<GuildRoleUpdatedEvent>();
			DiscordRole role = guildRoleUpdatedEvent.Role;
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildRoleUpdatedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildRoleUpdate Guild ID: ",
				guildRoleUpdatedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Role ID: ",
				guildRoleUpdatedEvent.Role.Id.ToString(),
				" Role Name: ",
				guildRoleUpdatedEvent.Role.Name
			}));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				DiscordRole discordRole = guild.Roles[role.Id];
				bool flag2 = discordRole != null;
				if (flag2)
				{
					DiscordRole discordRole2 = discordRole.UpdateRole(role);
					this._client.CallHook("OnDiscordGuildRoleUpdated", new object[]
					{
						discordRole,
						discordRole2,
						guild
					});
				}
				else
				{
					guild.Roles[role.Id] = role;
					this._logger.Warning(string.Concat(new string[]
					{
						"SocketListener.HandleDispatchGuildRoleUpdate Tried to update role that does not exists: ",
						guildRoleUpdatedEvent.Role.Name,
						"(",
						guildRoleUpdatedEvent.Role.Id.ToString(),
						")"
					}));
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005B9C File Offset: 0x00003D9C
		private void HandleDispatchGuildRoleDelete(EventPayload payload)
		{
			GuildRoleDeletedEvent guildRoleDeletedEvent = payload.EventData.ToObject<GuildRoleDeletedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildRoleDeletedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildRoleDelete Guild ID: ",
				guildRoleDeletedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Role ID: ",
				guildRoleDeletedEvent.RoleId.ToString()
			}));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				DiscordRole discordRole = guild.Roles[guildRoleDeletedEvent.RoleId];
				bool flag2 = discordRole != null;
				if (flag2)
				{
					guild.Roles.Remove(guildRoleDeletedEvent.RoleId);
					this._client.CallHook("OnDiscordGuildRoleDeleted", new object[]
					{
						discordRole,
						guild
					});
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005CA0 File Offset: 0x00003EA0
		private void HandleDispatchGuildScheduledEventCreate(EventPayload payload)
		{
			GuildScheduledEvent guildScheduledEvent = payload.EventData.ToObject<GuildScheduledEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildScheduledEvent.GuildId));
			bool flag = guild != null;
			if (flag)
			{
				guild.ScheduledEvents[guild.Id] = guildScheduledEvent;
			}
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildScheduledEventCreate Guild ID: ",
				guildScheduledEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Scheduled Event ID: ",
				guildScheduledEvent.Id.ToString()
			}));
			this._client.CallHook("OnDiscordGuildScheduledEventCreated", new object[]
			{
				guildScheduledEvent,
				guild
			});
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005D7C File Offset: 0x00003F7C
		private void HandleDispatchGuildScheduledEventUpdate(EventPayload payload)
		{
			GuildScheduledEvent guildScheduledEvent = payload.EventData.ToObject<GuildScheduledEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildScheduledEvent.GuildId));
			bool flag = guild != null;
			if (flag)
			{
				bool flag2 = guild.ScheduledEvents.ContainsKey(guildScheduledEvent.Id);
				if (flag2)
				{
					guild.ScheduledEvents[guildScheduledEvent.Id].Update(guildScheduledEvent);
				}
				else
				{
					guild.ScheduledEvents[guild.Id] = guildScheduledEvent;
				}
			}
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildScheduledEventUpdate Guild ID: ",
				guildScheduledEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Scheduled Event ID: ",
				guildScheduledEvent.Id.ToString()
			}));
			this._client.CallHook("OnDiscordGuildScheduledEventUpdated", new object[]
			{
				guildScheduledEvent,
				guild
			});
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005E8C File Offset: 0x0000408C
		private void HandleDispatchGuildScheduledEventDelete(EventPayload payload)
		{
			GuildScheduledEvent guildScheduledEvent = payload.EventData.ToObject<GuildScheduledEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildScheduledEvent.GuildId));
			guild.ScheduledEvents.Remove(guildScheduledEvent.Id);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildScheduledEventDelete Guild ID: ",
				guildScheduledEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Scheduled Event ID: ",
				guildScheduledEvent.Id.ToString()
			}));
			this._client.CallHook("OnDiscordGuildScheduledEventDeleted", new object[]
			{
				guildScheduledEvent,
				guild
			});
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00005F5C File Offset: 0x0000415C
		private void HandleDispatchGuildScheduledEventUserAdd(EventPayload payload)
		{
			GuildScheduleEventUserAddedEvent guildScheduleEventUserAddedEvent = payload.EventData.ToObject<GuildScheduleEventUserAddedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildScheduleEventUserAddedEvent.GuildId));
			GuildScheduledEvent guildScheduledEvent = guild.ScheduledEvents[guildScheduleEventUserAddedEvent.GuildScheduledEventId];
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildScheduledEventUserAdd Guild ID: ",
				guildScheduleEventUserAddedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" User ID: ",
				guildScheduleEventUserAddedEvent.UserId.ToString()
			}));
			this._client.CallHook("OnDiscordGuildScheduledEventUserAdded", new object[]
			{
				guildScheduleEventUserAddedEvent,
				guildScheduledEvent,
				guild
			});
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00006030 File Offset: 0x00004230
		private void HandleDispatchGuildScheduledEventUserRemove(EventPayload payload)
		{
			GuildScheduleEventUserRemovedEvent guildScheduleEventUserRemovedEvent = payload.EventData.ToObject<GuildScheduleEventUserRemovedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(guildScheduleEventUserRemovedEvent.GuildId));
			GuildScheduledEvent guildScheduledEvent = guild.ScheduledEvents[guildScheduleEventUserRemovedEvent.GuildScheduledEventId];
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchGuildScheduledEventUserRemove Guild ID: ",
				guildScheduleEventUserRemovedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" User ID: ",
				guildScheduleEventUserRemovedEvent.UserId.ToString()
			}));
			this._client.CallHook("OnDiscordGuildScheduledEventUserRemoved", new object[]
			{
				guildScheduleEventUserRemovedEvent,
				guildScheduledEvent,
				guild
			});
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006104 File Offset: 0x00004304
		private void HandleDispatchIntegrationCreate(EventPayload payload)
		{
			IntegrationCreatedEvent integrationCreatedEvent = payload.EventData.ToObject<IntegrationCreatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(integrationCreatedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchInteractionCreate Guild ID: ",
				integrationCreatedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Integration ID: ",
				integrationCreatedEvent.Id.ToString()
			}));
			this._client.CallHook("OnDiscordGuildIntegrationCreated", new object[]
			{
				integrationCreatedEvent,
				guild
			});
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000061C0 File Offset: 0x000043C0
		private void HandleDispatchIntegrationUpdate(EventPayload payload)
		{
			IntegrationUpdatedEvent integrationUpdatedEvent = payload.EventData.ToObject<IntegrationUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(integrationUpdatedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchIntegrationUpdate Guild ID: ",
				integrationUpdatedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Integration ID: ",
				integrationUpdatedEvent.Id.ToString()
			}));
			this._client.CallHook("OnDiscordGuildIntegrationUpdated", new object[]
			{
				integrationUpdatedEvent,
				guild
			});
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000627C File Offset: 0x0000447C
		private void HandleDispatchIntegrationDelete(EventPayload payload)
		{
			IntegrationDeletedEvent integrationDeletedEvent = payload.EventData.ToObject<IntegrationDeletedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(integrationDeletedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchIntegrationDelete Guild ID: ",
				integrationDeletedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Integration ID: ",
				integrationDeletedEvent.Id.ToString()
			}));
			this._client.CallHook("OnDiscordIntegrationDeleted", new object[]
			{
				integrationDeletedEvent,
				guild
			});
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00006338 File Offset: 0x00004538
		private void HandleDispatchMessageCreate(EventPayload payload)
		{
			DiscordMessage discordMessage = payload.EventData.ToObject<DiscordMessage>();
			DiscordGuild guild = this._client.GetGuild(discordMessage.GuildId);
			DiscordChannel channel = this._client.GetChannel(discordMessage.ChannelId, discordMessage.GuildId);
			bool flag = channel != null;
			if (flag)
			{
				channel.LastMessageId = new Snowflake?(discordMessage.Id);
			}
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageCreate: Guild ID: ",
				discordMessage.GuildId.ToString(),
				" Channel ID: ",
				discordMessage.ChannelId.ToString(),
				" Message ID: ",
				discordMessage.Id.ToString()
			}));
			bool flag2 = discordMessage.Author.Bot == null || !discordMessage.Author.Bot.Value;
			if (flag2)
			{
				bool flag3 = !string.IsNullOrEmpty(discordMessage.Content) && DiscordExtension.DiscordCommand.HasCommands() && ExtensionMethods.Contains<char>(DiscordExtension.DiscordConfig.Commands.CommandPrefixes, discordMessage.Content[0]);
				if (flag3)
				{
					string text;
					string[] array;
					discordMessage.Content.TrimStart(DiscordExtension.DiscordConfig.Commands.CommandPrefixes).ParseCommand(out text, out array);
					this._logger.Debug("SocketListener.HandleDispatchMessageCreate Cmd: " + text + " Args: " + string.Join(" ", array));
					bool flag4 = discordMessage.GuildId != null && discordMessage.GuildId.Value.IsValid() && DiscordExtension.DiscordCommand.HandleGuildCommand(this._client, discordMessage, channel, text, array);
					if (flag4)
					{
						this._logger.Debug("SocketListener.HandleDispatchMessageCreate Guild Handled Cmd: " + text);
						return;
					}
					bool flag5 = discordMessage.GuildId == null && DiscordExtension.DiscordCommand.HandleDirectMessageCommand(this._client, discordMessage, channel, text, array);
					if (flag5)
					{
						this._logger.Debug("SocketListener.HandleDispatchMessageCreate Direct Handled Cmd: " + text);
						return;
					}
				}
				bool flag6 = DiscordExtension.DiscordSubscriptions.HasSubscriptions() && channel != null && discordMessage.GuildId != null;
				if (flag6)
				{
					DiscordExtension.DiscordSubscriptions.HandleMessage(discordMessage, channel, this._client);
				}
			}
			bool flag7 = discordMessage.GuildId != null;
			if (flag7)
			{
				this._client.CallHook("OnDiscordGuildMessageCreated", new object[]
				{
					discordMessage,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageCreated", new object[]
				{
					discordMessage,
					channel
				});
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00006630 File Offset: 0x00004830
		private void HandleDispatchMessageUpdate(EventPayload payload)
		{
			DiscordMessage discordMessage = payload.EventData.ToObject<DiscordMessage>();
			DiscordGuild guild = this._client.GetGuild(discordMessage.GuildId);
			DiscordChannel channel = this._client.GetChannel(discordMessage.ChannelId, discordMessage.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageUpdate Guild ID: ",
				discordMessage.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				"  Channel ID: ",
				discordMessage.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Message ID: ",
				discordMessage.Id.ToString()
			}));
			bool flag = discordMessage.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildMessageUpdated", new object[]
				{
					discordMessage,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageUpdated", new object[]
				{
					discordMessage,
					channel
				});
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006778 File Offset: 0x00004978
		private void HandleDispatchMessageDelete(EventPayload payload)
		{
			MessageDeletedEvent messageDeletedEvent = payload.EventData.ToObject<MessageDeletedEvent>();
			DiscordGuild guild = this._client.GetGuild(messageDeletedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(messageDeletedEvent.ChannelId, messageDeletedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageDelete Message ID: ",
				messageDeletedEvent.Id.ToString(),
				" Channel ID: ",
				messageDeletedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Guild Id: ",
				messageDeletedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = messageDeletedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildMessageDeleted", new object[]
				{
					messageDeletedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageDeleted", new object[]
				{
					messageDeletedEvent,
					channel
				});
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000068C0 File Offset: 0x00004AC0
		private void HandleDispatchMessageDeleteBulk(EventPayload payload)
		{
			MessageBulkDeletedEvent messageBulkDeletedEvent = payload.EventData.ToObject<MessageBulkDeletedEvent>();
			DiscordGuild guild = this._client.GetGuild(messageBulkDeletedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(messageBulkDeletedEvent.ChannelId, messageBulkDeletedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageDeleteBulk Channel ID: ",
				messageBulkDeletedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Guild ID: ",
				messageBulkDeletedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = messageBulkDeletedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordDirectMessagesBulkDeleted", new object[]
				{
					messageBulkDeletedEvent.Ids,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessagesBulkDeleted", new object[]
				{
					messageBulkDeletedEvent.Ids,
					channel,
					guild
				});
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000069F4 File Offset: 0x00004BF4
		private void HandleDispatchMessageReactionAdd(EventPayload payload)
		{
			MessageReactionAddedEvent messageReactionAddedEvent = payload.EventData.ToObject<MessageReactionAddedEvent>();
			DiscordGuild guild = this._client.GetGuild(messageReactionAddedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(messageReactionAddedEvent.ChannelId, messageReactionAddedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageReactionAdd Emoji: ",
				messageReactionAddedEvent.Emoji.Name,
				" Channel ID: ",
				messageReactionAddedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Message ID: ",
				messageReactionAddedEvent.MessageId.ToString(),
				" User ID: ",
				messageReactionAddedEvent.UserId.ToString(),
				" Guild ID: ",
				messageReactionAddedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = messageReactionAddedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildMessageReactionAdded", new object[]
				{
					messageReactionAddedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageReactionAdded", new object[]
				{
					messageReactionAddedEvent,
					channel
				});
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00006B74 File Offset: 0x00004D74
		private void HandleDispatchMessageReactionRemove(EventPayload payload)
		{
			MessageReactionRemovedEvent messageReactionRemovedEvent = payload.EventData.ToObject<MessageReactionRemovedEvent>();
			DiscordGuild guild = this._client.GetGuild(messageReactionRemovedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(messageReactionRemovedEvent.ChannelId, messageReactionRemovedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageReactionRemove Emoji: ",
				messageReactionRemovedEvent.Emoji.Name,
				" Channel ID: ",
				messageReactionRemovedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Message ID: ",
				messageReactionRemovedEvent.MessageId.ToString(),
				" User ID: ",
				messageReactionRemovedEvent.UserId.ToString(),
				" Guild ID: ",
				messageReactionRemovedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = messageReactionRemovedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildMessageReactionRemoved", new object[]
				{
					messageReactionRemovedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageReactionRemoved", new object[]
				{
					messageReactionRemovedEvent,
					channel
				});
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00006CF4 File Offset: 0x00004EF4
		private void HandleDispatchMessageReactionRemoveAll(EventPayload payload)
		{
			MessageReactionRemovedAllEvent messageReactionRemovedAllEvent = payload.EventData.ToObject<MessageReactionRemovedAllEvent>();
			DiscordGuild guild = this._client.GetGuild(messageReactionRemovedAllEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(messageReactionRemovedAllEvent.ChannelId, messageReactionRemovedAllEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageReactionRemoveAll Channel ID: ",
				messageReactionRemovedAllEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Message ID: ",
				messageReactionRemovedAllEvent.MessageId.ToString(),
				" Guild ID: ",
				messageReactionRemovedAllEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = messageReactionRemovedAllEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildMessageReactionRemoved", new object[]
				{
					messageReactionRemovedAllEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageReactionRemoved", new object[]
				{
					messageReactionRemovedAllEvent,
					channel
				});
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006E3C File Offset: 0x0000503C
		private void HandleDispatchMessageReactionRemoveEmoji(EventPayload payload)
		{
			MessageReactionRemovedAllEmojiEvent messageReactionRemovedAllEmojiEvent = payload.EventData.ToObject<MessageReactionRemovedAllEmojiEvent>();
			DiscordGuild guild = this._client.GetGuild(messageReactionRemovedAllEmojiEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(messageReactionRemovedAllEmojiEvent.ChannelId, messageReactionRemovedAllEmojiEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchMessageReactionRemoveAll Emoji: ",
				messageReactionRemovedAllEmojiEvent.Emoji.Name,
				" Channel ID: ",
				messageReactionRemovedAllEmojiEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Message ID: ",
				messageReactionRemovedAllEmojiEvent.MessageId.ToString(),
				" Guild ID: ",
				messageReactionRemovedAllEmojiEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = messageReactionRemovedAllEmojiEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildMessageReactionEmojiRemoved", new object[]
				{
					messageReactionRemovedAllEmojiEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectMessageReactionEmojiRemoved", new object[]
				{
					messageReactionRemovedAllEmojiEvent,
					channel
				});
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006F9C File Offset: 0x0000519C
		private void HandleDispatchPresenceUpdate(EventPayload payload)
		{
			PresenceUpdatedEvent presenceUpdatedEvent = payload.EventData.ToObject<PresenceUpdatedEvent>();
			DiscordUser user = presenceUpdatedEvent.User;
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(presenceUpdatedEvent.GuildId));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				GuildMember guildMember = guild.Members[user.Id];
				bool flag2 = guildMember != null;
				if (flag2)
				{
					DiscordUser discordUser = guildMember.User.Update(user);
					this._logger.Verbose(string.Format("{0}.{1} Guild ID: {2} User ID: {3} Status: {4}", new object[]
					{
						"SocketListener",
						"HandleDispatchPresenceUpdate",
						presenceUpdatedEvent.GuildId.ToString(),
						presenceUpdatedEvent.User,
						presenceUpdatedEvent.Status.ToString()
					}));
					this._client.CallHook("OnDiscordGuildMemberPresenceUpdated", new object[]
					{
						presenceUpdatedEvent,
						guildMember,
						discordUser,
						guild
					});
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000070AC File Offset: 0x000052AC
		private void HandleDispatchTypingStart(EventPayload payload)
		{
			TypingStartedEvent typingStartedEvent = payload.EventData.ToObject<TypingStartedEvent>();
			DiscordGuild guild = this._client.GetGuild(typingStartedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(typingStartedEvent.ChannelId, typingStartedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchTypingStart Channel ID: ",
				typingStartedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" User ID: ",
				typingStartedEvent.UserId.ToString(),
				" Guild ID: ",
				typingStartedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null
			}));
			bool flag = typingStartedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildTypingStarted", new object[]
				{
					typingStartedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectTypingStarted", new object[]
				{
					typingStartedEvent,
					channel
				});
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000071F4 File Offset: 0x000053F4
		private void HandleDispatchUserUpdate(EventPayload payload)
		{
			DiscordUser discordUser = payload.EventData.ToObject<DiscordUser>();
			foreach (DiscordGuild discordGuild in this._client.Servers.Values)
			{
				bool isAvailable = discordGuild.IsAvailable;
				if (isAvailable)
				{
					GuildMember guildMember = discordGuild.Members[discordUser.Id];
					if (guildMember != null)
					{
						guildMember.User.Update(discordUser);
					}
				}
			}
			this._logger.Verbose("SocketListener.HandleDispatchUserUpdate User ID: " + discordUser.Id.ToString());
			this._client.CallHook("OnDiscordUserUpdated", new object[]
			{
				discordUser
			});
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000072D0 File Offset: 0x000054D0
		private void HandleDispatchVoiceStateUpdate(EventPayload payload)
		{
			VoiceState voiceState = payload.EventData.ToObject<VoiceState>();
			DiscordGuild guild = this._client.GetGuild(voiceState.GuildId);
			VoiceState voiceState2 = guild.VoiceStates[voiceState.UserId];
			DiscordChannel discordChannel = (voiceState.ChannelId != null) ? this._client.GetChannel(voiceState.ChannelId.Value, voiceState.GuildId) : null;
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchVoiceStateUpdate Guild ID: ",
				voiceState.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Channel ID: ",
				voiceState.ChannelId.ToString(),
				" Channel Name: ",
				(discordChannel != null) ? discordChannel.Name : null,
				" User ID: ",
				voiceState.UserId.ToString()
			}));
			bool flag = voiceState2 != null;
			if (flag)
			{
				voiceState2.Update(voiceState);
				voiceState = voiceState2;
			}
			else
			{
				guild.VoiceStates[voiceState.UserId] = voiceState;
			}
			bool flag2 = voiceState.GuildId != null;
			if (flag2)
			{
				this._client.CallHook("OnDiscordGuildVoiceStateUpdated", new object[]
				{
					voiceState,
					discordChannel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectVoiceStateUpdated", new object[]
				{
					voiceState,
					discordChannel
				});
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00007474 File Offset: 0x00005674
		private void HandleDispatchVoiceServerUpdate(EventPayload payload)
		{
			VoiceServerUpdatedEvent voiceServerUpdatedEvent = payload.EventData.ToObject<VoiceServerUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(voiceServerUpdatedEvent.GuildId));
			this._logger.Verbose("SocketListener.HandleDispatchVoiceServerUpdate Guild ID: " + voiceServerUpdatedEvent.GuildId.ToString() + " Guild Name: " + ((guild != null) ? guild.Name : null));
			this._client.CallHook("OnDiscordGuildVoiceServerUpdated", new object[]
			{
				voiceServerUpdatedEvent,
				guild
			});
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00007500 File Offset: 0x00005700
		private void HandleDispatchWebhooksUpdate(EventPayload payload)
		{
			WebhooksUpdatedEvent webhooksUpdatedEvent = payload.EventData.ToObject<WebhooksUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(webhooksUpdatedEvent.GuildId));
			DiscordChannel channel = this._client.GetChannel(webhooksUpdatedEvent.ChannelId, new Snowflake?(webhooksUpdatedEvent.GuildId));
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchWebhooksUpdate Guild ID: ",
				webhooksUpdatedEvent.GuildId.ToString(),
				" Guild Name ",
				(guild != null) ? guild.Name : null,
				" Channel ID: ",
				webhooksUpdatedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null
			}));
			this._client.CallHook("OnDiscordGuildWebhookUpdated", new object[]
			{
				webhooksUpdatedEvent,
				channel,
				guild
			});
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000075F4 File Offset: 0x000057F4
		private void HandleDispatchInviteCreate(EventPayload payload)
		{
			InviteCreatedEvent inviteCreatedEvent = payload.EventData.ToObject<InviteCreatedEvent>();
			DiscordGuild guild = this._client.GetGuild(inviteCreatedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(inviteCreatedEvent.ChannelId, inviteCreatedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchInviteCreate Guild ID: ",
				inviteCreatedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Channel ID: ",
				inviteCreatedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Code: ",
				inviteCreatedEvent.Code
			}));
			bool flag = inviteCreatedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildInviteCreated", new object[]
				{
					inviteCreatedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectInviteCreated", new object[]
				{
					inviteCreatedEvent,
					channel
				});
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000772C File Offset: 0x0000592C
		private void HandleDispatchInviteDelete(EventPayload payload)
		{
			InviteDeletedEvent inviteDeletedEvent = payload.EventData.ToObject<InviteDeletedEvent>();
			DiscordGuild guild = this._client.GetGuild(inviteDeletedEvent.GuildId);
			DiscordChannel channel = this._client.GetChannel(inviteDeletedEvent.ChannelId, inviteDeletedEvent.GuildId);
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchInviteDelete Guild ID: ",
				inviteDeletedEvent.GuildId.ToString(),
				" Guild Name: ",
				(guild != null) ? guild.Name : null,
				" Channel ID: ",
				inviteDeletedEvent.ChannelId.ToString(),
				" Channel Name: ",
				(channel != null) ? channel.Name : null,
				" Code: ",
				inviteDeletedEvent.Code
			}));
			bool flag = inviteDeletedEvent.GuildId != null;
			if (flag)
			{
				this._client.CallHook("OnDiscordGuildInviteDeleted", new object[]
				{
					inviteDeletedEvent,
					channel,
					guild
				});
			}
			else
			{
				this._client.CallHook("OnDiscordDirectInviteDeleted", new object[]
				{
					inviteDeletedEvent,
					channel
				});
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00007864 File Offset: 0x00005A64
		private void HandleDispatchInteractionCreate(EventPayload payload)
		{
			DiscordInteraction discordInteraction = payload.EventData.ToObject<DiscordInteraction>();
			this._logger.Verbose(string.Concat(new string[]
			{
				"SocketListener.HandleDispatchInteractionCreate Guild ID: ",
				discordInteraction.GuildId.ToString(),
				" Channel ID: ",
				discordInteraction.ChannelId.ToString(),
				" Interaction ID: ",
				discordInteraction.Id.ToString(),
				" Interaction Token: ",
				discordInteraction.Token
			}));
			this._client.CallHook("OnDiscordInteractionCreated", new object[]
			{
				discordInteraction
			});
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000791E File Offset: 0x00005B1E
		private void HandleGuildJoinRequestDelete(EventPayload payload)
		{
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00007924 File Offset: 0x00005B24
		private void HandleDispatchThreadCreated(EventPayload payload)
		{
			DiscordChannel discordChannel = payload.EventData.ToObject<DiscordChannel>();
			bool flag = discordChannel.GuildId != null;
			if (flag)
			{
				DiscordGuild guild = this._client.GetGuild(discordChannel.GuildId);
				bool flag2 = guild != null && guild.IsAvailable;
				if (flag2)
				{
					this._logger.Verbose(string.Concat(new string[]
					{
						"SocketListener.HandleDispatchThreadCreated Guild: ",
						guild.Name,
						"(",
						guild.Id.ToString(),
						") Thread: ",
						discordChannel.Name,
						"(",
						discordChannel.Id.ToString(),
						")"
					}));
					guild.Threads[discordChannel.Id] = discordChannel;
					this._client.CallHook("OnDiscordGuildThreadCreated", new object[]
					{
						discordChannel,
						guild
					});
				}
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00007A34 File Offset: 0x00005C34
		private void HandleDispatchThreadUpdated(EventPayload payload)
		{
			DiscordChannel discordChannel = payload.EventData.ToObject<DiscordChannel>();
			bool flag = discordChannel.GuildId != null;
			if (flag)
			{
				DiscordGuild guild = this._client.GetGuild(discordChannel.GuildId);
				bool flag2 = guild != null && guild.IsAvailable;
				if (flag2)
				{
					this._logger.Verbose(string.Concat(new string[]
					{
						"SocketListener.HandleDispatchThreadUpdated Guild: ",
						guild.Name,
						"(",
						guild.Id.ToString(),
						") Thread: ",
						discordChannel.Name,
						"(",
						discordChannel.Id.ToString(),
						")"
					}));
					DiscordChannel discordChannel2 = guild.Threads[discordChannel.Id];
					bool flag3 = discordChannel2 != null;
					if (flag3)
					{
						DiscordChannel discordChannel3 = discordChannel2.Update(discordChannel);
						this._client.CallHook("OnDiscordGuildThreadUpdated", new object[]
						{
							discordChannel,
							discordChannel3,
							guild
						});
					}
					else
					{
						this._logger.Warning(string.Concat(new string[]
						{
							"SocketListener.HandleDispatchThreadUpdated Tried to update a thread that doesn't exist. Guild: ",
							guild.Name,
							"(",
							guild.Id.ToString(),
							") Thread: ",
							discordChannel.Name,
							"(",
							discordChannel.Id.ToString(),
							")"
						}));
					}
				}
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00007BE8 File Offset: 0x00005DE8
		private void HandleDispatchThreadDeleted(EventPayload payload)
		{
			DiscordChannel discordChannel = payload.EventData.ToObject<DiscordChannel>();
			bool flag = discordChannel.GuildId != null;
			if (flag)
			{
				DiscordGuild guild = this._client.GetGuild(discordChannel.GuildId);
				bool flag2 = guild != null && guild.IsAvailable;
				if (flag2)
				{
					this._logger.Verbose(string.Concat(new string[]
					{
						"SocketListener.HandleDispatchThreadDeleted Guild: ",
						guild.Name,
						"(",
						guild.Id.ToString(),
						") Thread: ",
						discordChannel.Name,
						"(",
						discordChannel.Id.ToString(),
						")"
					}));
					discordChannel = (guild.Threads[discordChannel.Id] ?? discordChannel);
					guild.Threads.Remove(discordChannel.Id);
					this._client.CallHook("OnDiscordGuildThreadDeleted", new object[]
					{
						discordChannel,
						guild
					});
				}
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00007D0C File Offset: 0x00005F0C
		private void HandleDispatchThreadListSync(EventPayload payload)
		{
			ThreadListSyncEvent threadListSyncEvent = payload.EventData.ToObject<ThreadListSyncEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(threadListSyncEvent.GuildId));
			List<Snowflake> list = new List<Snowflake>();
			foreach (DiscordChannel discordChannel in guild.Threads.Values)
			{
				bool flag = discordChannel.ParentId != null && (threadListSyncEvent.ChannelIds == null || threadListSyncEvent.ChannelIds.Contains(discordChannel.ParentId.Value)) && !threadListSyncEvent.Threads.ContainsKey(discordChannel.Id);
				if (flag)
				{
					list.Add(discordChannel.Id);
				}
			}
			foreach (Snowflake snowflake in list)
			{
				guild.Threads.Remove(snowflake);
			}
			foreach (DiscordChannel discordChannel2 in threadListSyncEvent.Threads.Values)
			{
				DiscordChannel discordChannel3 = guild.Threads[discordChannel2.Id];
				bool flag2 = discordChannel3 != null;
				if (flag2)
				{
					discordChannel3.Update(discordChannel2);
					discordChannel3.ThreadMembers.Clear();
				}
				else
				{
					guild.Threads[discordChannel2.Id] = discordChannel2;
				}
			}
			foreach (ThreadMember threadMember in threadListSyncEvent.Members)
			{
				bool flag3 = threadMember.Id != null && threadMember.UserId != null;
				if (flag3)
				{
					DiscordChannel discordChannel4 = guild.Threads[threadMember.Id.Value];
					bool flag4 = discordChannel4 != null;
					if (flag4)
					{
						discordChannel4.ThreadMembers[threadMember.UserId.Value] = threadMember;
					}
				}
			}
			this._client.CallHook("OnDiscordGuildThreadListSynced", new object[]
			{
				threadListSyncEvent,
				guild
			});
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007FA8 File Offset: 0x000061A8
		private void HandleDispatchThreadMemberUpdated(EventPayload payload)
		{
			ThreadMemberUpdateEvent threadMemberUpdateEvent = payload.EventData.ToObject<ThreadMemberUpdateEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(threadMemberUpdateEvent.GuildId));
			bool flag = guild == null;
			if (!flag)
			{
				bool flag2 = threadMemberUpdateEvent.Id == null || threadMemberUpdateEvent.UserId == null;
				if (!flag2)
				{
					DiscordChannel discordChannel = guild.Threads[threadMemberUpdateEvent.Id.Value];
					bool flag3 = discordChannel == null;
					if (!flag3)
					{
						ThreadMember threadMember = discordChannel.ThreadMembers[threadMemberUpdateEvent.UserId.Value];
						bool flag4 = threadMember != null;
						if (flag4)
						{
							threadMember.Update(threadMemberUpdateEvent);
						}
						else
						{
							discordChannel.ThreadMembers[threadMemberUpdateEvent.UserId.Value] = threadMemberUpdateEvent;
						}
						this._client.CallHook("OnDiscordGuildThreadMemberUpdated", new object[]
						{
							threadMemberUpdateEvent,
							discordChannel,
							guild
						});
					}
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000080B8 File Offset: 0x000062B8
		private void HandleDispatchThreadMembersUpdated(EventPayload payload)
		{
			ThreadMembersUpdatedEvent threadMembersUpdatedEvent = payload.EventData.ToObject<ThreadMembersUpdatedEvent>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(threadMembersUpdatedEvent.GuildId));
			bool flag = guild != null && guild.IsAvailable;
			if (flag)
			{
				DiscordChannel discordChannel = guild.Threads[threadMembersUpdatedEvent.Id];
				bool flag2 = discordChannel != null;
				if (flag2)
				{
					bool flag3 = threadMembersUpdatedEvent.AddedMembers != null;
					if (flag3)
					{
						foreach (ThreadMember threadMember in threadMembersUpdatedEvent.AddedMembers)
						{
							bool flag4 = threadMember.UserId != null;
							if (flag4)
							{
								discordChannel.ThreadMembers[threadMember.UserId.Value] = threadMember;
							}
						}
					}
					bool flag5 = threadMembersUpdatedEvent.RemovedMemberIds != null;
					if (flag5)
					{
						foreach (Snowflake snowflake in threadMembersUpdatedEvent.RemovedMemberIds)
						{
							discordChannel.ThreadMembers.Remove(snowflake);
						}
					}
					this._client.CallHook("OnDiscordGuildThreadMembersUpdated", new object[]
					{
						threadMembersUpdatedEvent,
						guild
					});
				}
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000822C File Offset: 0x0000642C
		private void HandleDispatchStageInstanceCreated(EventPayload payload)
		{
			StageInstance stageInstance = payload.EventData.ToObject<StageInstance>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(stageInstance.GuildId));
			bool flag = guild != null;
			if (flag)
			{
				guild.StageInstances[stageInstance.Id] = stageInstance;
				this._client.CallHook("OnDiscordStageInstanceCreated", new object[]
				{
					stageInstance,
					guild
				});
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000829C File Offset: 0x0000649C
		private void HandleDispatchStageInstanceUpdated(EventPayload payload)
		{
			StageInstance stageInstance = payload.EventData.ToObject<StageInstance>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(stageInstance.GuildId));
			bool flag = guild != null;
			if (flag)
			{
				StageInstance stageInstance2 = guild.StageInstances[stageInstance.Id];
				bool flag2 = stageInstance2 == null;
				if (flag2)
				{
					guild.StageInstances[stageInstance.Id] = stageInstance;
					this._client.CallHook("OnDiscordStageInstanceUpdated", new object[]
					{
						stageInstance,
						stageInstance,
						guild
					});
				}
				else
				{
					StageInstance stageInstance3 = stageInstance2.Update(stageInstance);
					this._client.CallHook("OnDiscordStageInstanceUpdated", new object[]
					{
						stageInstance,
						stageInstance3,
						guild
					});
				}
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00008360 File Offset: 0x00006560
		private void HandleDispatchStageInstanceDeleted(EventPayload payload)
		{
			StageInstance stageInstance = payload.EventData.ToObject<StageInstance>();
			DiscordGuild guild = this._client.GetGuild(new Snowflake?(stageInstance.GuildId));
			bool flag = guild != null;
			if (flag)
			{
				StageInstance stageInstance2 = guild.StageInstances[stageInstance.Id];
				guild.StageInstances.Remove(stageInstance.Id);
				guild.StageInstances[stageInstance.Id] = stageInstance;
				this._client.CallHook("OnDiscordStageInstanceDeleted", new object[]
				{
					stageInstance2 ?? stageInstance,
					guild
				});
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000083F6 File Offset: 0x000065F6
		private void HandleDispatchUnhandledEvent(EventPayload payload)
		{
			this._logger.Verbose(string.Format("Unhandled Dispatch Event: {0}.\n{1}", payload.EventName, JsonConvert.SerializeObject(payload)));
			this._client.CallHook("OnDiscordUnhandledCommand", new object[]
			{
				payload
			});
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00008436 File Offset: 0x00006636
		private void HandleHeartbeat(EventPayload payload)
		{
			this._logger.Debug("Manually sent heartbeat (received opcode 1)");
			this.SendHeartbeat();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00008451 File Offset: 0x00006651
		private void HandleReconnect(EventPayload payload)
		{
			this._logger.Info("Discord has requested a reconnect. Reconnecting...");
			this._webSocket.Disconnect(true, true, true);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00008474 File Offset: 0x00006674
		private void HandleInvalidSession(EventPayload payload)
		{
			bool flag;
			if (!string.IsNullOrEmpty(this._sessionId))
			{
				JToken tokenData = payload.TokenData;
				flag = (tokenData != null && tokenData.ToObject<bool>());
			}
			else
			{
				flag = false;
			}
			bool shouldResume = flag;
			this._logger.Warning("Invalid Session ID opcode received! Attempting to reconnect. Should Resume? " + shouldResume.ToString());
			this._webSocket.Disconnect(true, shouldResume, false);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000084D4 File Offset: 0x000066D4
		private void HandleHello(EventPayload payload)
		{
			GatewayHelloEvent gatewayHelloEvent = payload.EventData.ToObject<GatewayHelloEvent>();
			this._heartbeat.SetupHeartbeat((float)gatewayHelloEvent.HeartbeatInterval);
			bool flag = this._webSocket.ShouldAttemptResume && !string.IsNullOrEmpty(this._sessionId);
			if (flag)
			{
				this._logger.Info("SocketListener.HandleHello Attempting to resume session with ID: " + this._sessionId);
				this.Resume(this._sessionId, this._sequence);
			}
			else
			{
				this._logger.Debug("SocketListener.HandleHello Identifying bot with discord.");
				this.Identify();
				this._webSocket.ShouldAttemptResume = true;
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000857C File Offset: 0x0000677C
		private void HandleHeartbeatAcknowledge(EventPayload payload)
		{
			this._heartbeat.HeartbeatAcknowledged = true;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000858C File Offset: 0x0000678C
		private void UnhandledOpCode(EventPayload payload)
		{
			this._logger.Warning("Unhandled OP code: " + payload.OpCode.ToString() + ". Please contact Discord Extension authors.");
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000085C9 File Offset: 0x000067C9
		internal void SendHeartbeat()
		{
			this._webSocket.Send(GatewayCommandCode.Heartbeat, this._sequence);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000085E4 File Offset: 0x000067E4
		private void Identify()
		{
			bool flag = !this._client.Initialized;
			if (!flag)
			{
				IdentifyCommand data = new IdentifyCommand
				{
					Token = this._client.Settings.ApiToken,
					Properties = new Properties
					{
						OS = "Oxide.Ext.Discord",
						Browser = "Oxide.Ext.Discord",
						Device = "Oxide.Ext.Discord"
					},
					Intents = this._client.Settings.Intents,
					Compress = new bool?(false),
					LargeThreshold = new int?(50),
					Shard = new List<int>
					{
						0,
						1
					}
				};
				this._webSocket.Send(GatewayCommandCode.Identify, data);
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000086AC File Offset: 0x000068AC
		private void Resume(string sessionId, int sequence)
		{
			bool flag = !this._client.Initialized;
			if (!flag)
			{
				ResumeSessionCommand data = new ResumeSessionCommand
				{
					Sequence = sequence,
					SessionId = sessionId,
					Token = this._client.Settings.ApiToken
				};
				this._webSocket.Send(GatewayCommandCode.Resume, data);
			}
		}

		// Token: 0x04000077 RID: 119
		private string _sessionId;

		// Token: 0x04000078 RID: 120
		private int _sequence;

		// Token: 0x0400007A RID: 122
		private readonly BotClient _client;

		// Token: 0x0400007B RID: 123
		private readonly Socket _webSocket;

		// Token: 0x0400007C RID: 124
		private readonly ILogger _logger;

		// Token: 0x0400007D RID: 125
		private readonly SocketCommandHandler _commands;

		// Token: 0x0400007E RID: 126
		private HeartbeatHandler _heartbeat;
	}
}
