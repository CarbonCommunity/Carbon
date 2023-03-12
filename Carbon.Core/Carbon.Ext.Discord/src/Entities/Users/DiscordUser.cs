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
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users.Connections;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Users
{
	// Token: 0x0200004B RID: 75
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordUser : ISnowflakeEntity
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600023E RID: 574 RVA: 0x0000EA2C File Offset: 0x0000CC2C
		// (set) Token: 0x0600023F RID: 575 RVA: 0x0000EA34 File Offset: 0x0000CC34
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000240 RID: 576 RVA: 0x0000EA3D File Offset: 0x0000CC3D
		// (set) Token: 0x06000241 RID: 577 RVA: 0x0000EA45 File Offset: 0x0000CC45
		[JsonProperty("username")]
		public string Username { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000242 RID: 578 RVA: 0x0000EA4E File Offset: 0x0000CC4E
		// (set) Token: 0x06000243 RID: 579 RVA: 0x0000EA56 File Offset: 0x0000CC56
		[JsonProperty("discriminator")]
		public string Discriminator { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000244 RID: 580 RVA: 0x0000EA5F File Offset: 0x0000CC5F
		// (set) Token: 0x06000245 RID: 581 RVA: 0x0000EA67 File Offset: 0x0000CC67
		[JsonProperty("avatar")]
		public string Avatar { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000246 RID: 582 RVA: 0x0000EA70 File Offset: 0x0000CC70
		// (set) Token: 0x06000247 RID: 583 RVA: 0x0000EA78 File Offset: 0x0000CC78
		[JsonProperty("bot")]
		public bool? Bot { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000248 RID: 584 RVA: 0x0000EA81 File Offset: 0x0000CC81
		// (set) Token: 0x06000249 RID: 585 RVA: 0x0000EA89 File Offset: 0x0000CC89
		[JsonProperty("system")]
		public bool? System { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000EA92 File Offset: 0x0000CC92
		// (set) Token: 0x0600024B RID: 587 RVA: 0x0000EA9A File Offset: 0x0000CC9A
		[JsonProperty("mfa_enabled")]
		public bool? MfaEnabled { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600024C RID: 588 RVA: 0x0000EAA3 File Offset: 0x0000CCA3
		// (set) Token: 0x0600024D RID: 589 RVA: 0x0000EAAB File Offset: 0x0000CCAB
		[JsonProperty("banner")]
		public string Banner { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000EAB4 File Offset: 0x0000CCB4
		// (set) Token: 0x0600024F RID: 591 RVA: 0x0000EABC File Offset: 0x0000CCBC
		[JsonProperty("accent_color")]
		public DiscordColor? AccentColor { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000250 RID: 592 RVA: 0x0000EAC5 File Offset: 0x0000CCC5
		// (set) Token: 0x06000251 RID: 593 RVA: 0x0000EACD File Offset: 0x0000CCCD
		[JsonProperty("locale")]
		public string Locale { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000252 RID: 594 RVA: 0x0000EAD6 File Offset: 0x0000CCD6
		// (set) Token: 0x06000253 RID: 595 RVA: 0x0000EADE File Offset: 0x0000CCDE
		[JsonProperty("verified")]
		public bool? Verified { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000EAE7 File Offset: 0x0000CCE7
		// (set) Token: 0x06000255 RID: 597 RVA: 0x0000EAEF File Offset: 0x0000CCEF
		[JsonProperty("email")]
		public string Email { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000EAF8 File Offset: 0x0000CCF8
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000EB00 File Offset: 0x0000CD00
		[JsonProperty("flags")]
		public UserFlags? Flags { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000EB09 File Offset: 0x0000CD09
		// (set) Token: 0x06000259 RID: 601 RVA: 0x0000EB11 File Offset: 0x0000CD11
		[JsonProperty("premium_type")]
		public UserPremiumType? PremiumType { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000EB1A File Offset: 0x0000CD1A
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000EB22 File Offset: 0x0000CD22
		[JsonProperty("public_flags")]
		public UserFlags? PublicFlags { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0000EB2B File Offset: 0x0000CD2B
		public string Mention
		{
			get
			{
				return DiscordFormatting.MentionUserNickname(this.Id);
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600025D RID: 605 RVA: 0x0000EB38 File Offset: 0x0000CD38
		public string MentionUser
		{
			get
			{
				return DiscordFormatting.MentionUser(this.Id);
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000EB45 File Offset: 0x0000CD45
		public string GetDefaultAvatarUrl
		{
			get
			{
				return DiscordCdn.GetUserDefaultAvatarUrl(this.Id, this.Discriminator);
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600025F RID: 607 RVA: 0x0000EB58 File Offset: 0x0000CD58
		public string GetAvatarUrl
		{
			get
			{
				return DiscordCdn.GetUserAvatarUrl(this.Id, this.Avatar, ImageFormat.Auto);
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0000EB6C File Offset: 0x0000CD6C
		public string GetFullUserName
		{
			get
			{
				return this.Username + "#" + this.Discriminator;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000261 RID: 609 RVA: 0x0000EB84 File Offset: 0x0000CD84
		public IPlayer Player
		{
			get
			{
				return DiscordExtension.DiscordLink.GetPlayer(this.Id);
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000EB96 File Offset: 0x0000CD96
		public static void GetCurrentUser(DiscordClient client, Action<DiscordUser> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordUser>("/users/@me", RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000EBB4 File Offset: 0x0000CDB4
		public static void GetUser(DiscordClient client, Snowflake userId, Action<DiscordUser> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest<DiscordUser>(string.Format("/users/{0}", userId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000EC00 File Offset: 0x0000CE00
		public void SendDirectMessage(DiscordClient client, MessageCreate message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			DiscordUser.CreateDirectMessageChannel(client, this.Id, delegate(DiscordChannel channel)
			{
				channel.CreateMessage(client, message, callback, error);
			}, null);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000EC50 File Offset: 0x0000CE50
		public void SendDirectMessage(DiscordClient client, string message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			DiscordUser.CreateDirectMessageChannel(client, this.Id, delegate(DiscordChannel channel)
			{
				channel.CreateMessage(client, message, callback, error);
			}, null);
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000ECA0 File Offset: 0x0000CEA0
		public void SendDirectMessage(DiscordClient client, List<DiscordEmbed> embeds, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			DiscordUser.CreateDirectMessageChannel(client, this.Id, delegate(DiscordChannel channel)
			{
				channel.CreateMessage(client, embeds, callback, error);
			}, null);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000ECF0 File Offset: 0x0000CEF0
		public void ModifyCurrentUser(DiscordClient client, Action<DiscordUser> callback = null, Action<RestError> error = null)
		{
			this.ModifyCurrentUser(client, this.Username, this.Avatar, callback, error);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000ED08 File Offset: 0x0000CF08
		public void ModifyCurrentUser(DiscordClient client, string username = "", string avatarData = "", Action<DiscordUser> callback = null, Action<RestError> error = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["username"] = username;
			dictionary["avatar"] = avatarData;
			Dictionary<string, string> data = dictionary;
			client.Bot.Rest.DoRequest<DiscordUser>("/users/@me", RequestMethod.PATCH, data, callback, error);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000ED52 File Offset: 0x0000CF52
		public void GetCurrentUserGuilds(DiscordClient client, UserGuildsRequest request = null, Action<List<DiscordGuild>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordGuild>>("/users/@me/guilds" + ((request != null) ? request.ToQueryString() : null), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000ED81 File Offset: 0x0000CF81
		public void LeaveGuild(DiscordClient client, DiscordGuild guild, Action callback = null, Action<RestError> error = null)
		{
			this.LeaveGuild(client, guild.Id, callback, error);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000ED94 File Offset: 0x0000CF94
		public void LeaveGuild(DiscordClient client, Snowflake guildId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest(string.Format("/users/@me/guilds/{0}", guildId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000EDE1 File Offset: 0x0000CFE1
		public void CreateDirectMessageChannel(DiscordClient client, Action<DiscordChannel> callback, Action<RestError> error = null)
		{
			DiscordUser.CreateDirectMessageChannel(client, this.Id, callback, error);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000EDF4 File Offset: 0x0000CFF4
		public static void CreateDirectMessageChannel(DiscordClient client, Snowflake userId, Action<DiscordChannel> callback, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			bool flag2 = userId == client.Bot.BotUser.Id;
			if (flag2)
			{
				client.Logger.Error("Tried to create a direct message to the bot which is not allowed.");
			}
			else
			{
				DiscordChannel discordChannel = client.Bot.DirectMessagesByUserId[userId];
				bool flag3 = discordChannel != null;
				if (flag3)
				{
					callback(discordChannel);
				}
				else
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary["recipient_id"] = userId;
					Dictionary<string, object> data = dictionary;
					client.Bot.Rest.DoRequest<DiscordChannel>("/users/@me/channels", RequestMethod.POST, data, delegate(DiscordChannel newChannel)
					{
						client.Bot.AddDirectChannel(newChannel);
						Action<DiscordChannel> callback2 = callback;
						if (callback2 != null)
						{
							callback2(newChannel);
						}
					}, error);
				}
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000EEDC File Offset: 0x0000D0DC
		public void CreateGroupDm(DiscordClient client, string[] accessTokens, Hash<Snowflake, string> nicks, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["access_tokens"] = accessTokens;
			dictionary["nicks"] = nicks;
			Dictionary<string, object> data = dictionary;
			client.Bot.Rest.DoRequest<DiscordChannel>("/users/@me/channels", RequestMethod.POST, data, callback, error);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000EF26 File Offset: 0x0000D126
		public void GetUserConnections(DiscordClient client, Action<List<Connection>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<Connection>>("/users/@me/connections", RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000EF43 File Offset: 0x0000D143
		public void GroupDmAddRecipient(DiscordClient client, DiscordChannel channel, string accessToken, Action callback = null, Action<RestError> error = null)
		{
			this.GroupDmAddRecipient(client, channel.Id, accessToken, this.Username, callback, error);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000EF60 File Offset: 0x0000D160
		public void GroupDmAddRecipient(DiscordClient client, Snowflake channelId, string accessToken, string nick, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["access_token"] = accessToken;
			dictionary["nick"] = nick;
			Dictionary<string, string> data = dictionary;
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/recipients/{1}", channelId, this.Id), RequestMethod.PUT, data, callback, error);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000EFDA File Offset: 0x0000D1DA
		public void GroupDmRemoveRecipient(DiscordClient client, DiscordChannel channel, Action callback = null, Action<RestError> error = null)
		{
			this.GroupDmRemoveRecipient(client, channel.Id, callback, error);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000EFF0 File Offset: 0x0000D1F0
		public void GroupDmRemoveRecipient(DiscordClient client, Snowflake channelId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/recipients/{1}", channelId, this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000F048 File Offset: 0x0000D248
		internal DiscordUser Update(DiscordUser update)
		{
			DiscordUser result = (DiscordUser)base.MemberwiseClone();
			bool flag = update.Username != null;
			if (flag)
			{
				this.Username = update.Username;
			}
			bool flag2 = update.Discriminator != null;
			if (flag2)
			{
				this.Discriminator = update.Discriminator;
			}
			bool flag3 = update.Avatar != null;
			if (flag3)
			{
				this.Avatar = update.Avatar;
			}
			bool flag4 = update.Bot != null;
			if (flag4)
			{
				this.Bot = update.Bot;
			}
			bool flag5 = update.MfaEnabled != null;
			if (flag5)
			{
				this.MfaEnabled = update.MfaEnabled;
			}
			bool flag6 = update.Locale != null;
			if (flag6)
			{
				this.Locale = update.Locale;
			}
			bool flag7 = update.Verified != null;
			if (flag7)
			{
				this.Verified = update.Verified;
			}
			bool flag8 = update.Email != null;
			if (flag8)
			{
				this.Email = update.Email;
			}
			bool flag9 = update.Flags != null;
			if (flag9)
			{
				this.Flags = update.Flags;
			}
			bool flag10 = update.PremiumType != null;
			if (flag10)
			{
				this.PremiumType = update.PremiumType;
			}
			bool flag11 = update.PublicFlags != null;
			if (flag11)
			{
				this.PublicFlags = update.PublicFlags;
			}
			return result;
		}
	}
}
