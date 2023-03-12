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
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Channels.Stages;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Entities.Invites;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000FC RID: 252
	[JsonObject(MemberSerialization = ( MemberSerialization )1 )]
	public class DiscordChannel : ISnowflakeEntity
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060008D6 RID: 2262 RVA: 0x00015F6B File Offset: 0x0001416B
		// (set) Token: 0x060008D7 RID: 2263 RVA: 0x00015F73 File Offset: 0x00014173
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060008D8 RID: 2264 RVA: 0x00015F7C File Offset: 0x0001417C
		// (set) Token: 0x060008D9 RID: 2265 RVA: 0x00015F84 File Offset: 0x00014184
		[JsonProperty("type")]
		public ChannelType Type { get; set; }

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060008DA RID: 2266 RVA: 0x00015F8D File Offset: 0x0001418D
		// (set) Token: 0x060008DB RID: 2267 RVA: 0x00015F95 File Offset: 0x00014195
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060008DC RID: 2268 RVA: 0x00015F9E File Offset: 0x0001419E
		// (set) Token: 0x060008DD RID: 2269 RVA: 0x00015FA6 File Offset: 0x000141A6
		[JsonProperty("position")]
		public int? Position { get; set; }

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x00015FAF File Offset: 0x000141AF
		// (set) Token: 0x060008DF RID: 2271 RVA: 0x00015FB7 File Offset: 0x000141B7
		[JsonConverter(typeof(HashListConverter<Overwrite>))]
		[JsonProperty("permission_overwrites")]
		public Hash<Snowflake, Overwrite> PermissionOverwrites { get; set; }

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060008E0 RID: 2272 RVA: 0x00015FC0 File Offset: 0x000141C0
		// (set) Token: 0x060008E1 RID: 2273 RVA: 0x00015FC8 File Offset: 0x000141C8
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060008E2 RID: 2274 RVA: 0x00015FD1 File Offset: 0x000141D1
		// (set) Token: 0x060008E3 RID: 2275 RVA: 0x00015FD9 File Offset: 0x000141D9
		[JsonProperty("topic")]
		public string Topic { get; set; }

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x00015FE2 File Offset: 0x000141E2
		// (set) Token: 0x060008E5 RID: 2277 RVA: 0x00015FEA File Offset: 0x000141EA
		[JsonProperty("nsfw")]
		public bool? Nsfw { get; set; }

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060008E6 RID: 2278 RVA: 0x00015FF3 File Offset: 0x000141F3
		// (set) Token: 0x060008E7 RID: 2279 RVA: 0x00015FFB File Offset: 0x000141FB
		[JsonProperty("last_message_id")]
		public Snowflake? LastMessageId { get; set; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x00016004 File Offset: 0x00014204
		// (set) Token: 0x060008E9 RID: 2281 RVA: 0x0001600C File Offset: 0x0001420C
		[JsonProperty("bitrate")]
		public int? Bitrate { get; set; }

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060008EA RID: 2282 RVA: 0x00016015 File Offset: 0x00014215
		// (set) Token: 0x060008EB RID: 2283 RVA: 0x0001601D File Offset: 0x0001421D
		[JsonProperty("user_limit")]
		public int? UserLimit { get; set; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060008EC RID: 2284 RVA: 0x00016026 File Offset: 0x00014226
		// (set) Token: 0x060008ED RID: 2285 RVA: 0x0001602E File Offset: 0x0001422E
		[JsonProperty("rate_limit_per_user")]
		public int? RateLimitPerUser { get; set; }

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x00016037 File Offset: 0x00014237
		// (set) Token: 0x060008EF RID: 2287 RVA: 0x0001603F File Offset: 0x0001423F
		[JsonConverter(typeof(HashListConverter<DiscordUser>))]
		[JsonProperty("recipients")]
		public Hash<Snowflake, DiscordUser> Recipients { get; set; }

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x00016048 File Offset: 0x00014248
		// (set) Token: 0x060008F1 RID: 2289 RVA: 0x00016050 File Offset: 0x00014250
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060008F2 RID: 2290 RVA: 0x00016059 File Offset: 0x00014259
		// (set) Token: 0x060008F3 RID: 2291 RVA: 0x00016061 File Offset: 0x00014261
		[JsonProperty("owner_id")]
		public Snowflake? OwnerId { get; set; }

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0001606A File Offset: 0x0001426A
		// (set) Token: 0x060008F5 RID: 2293 RVA: 0x00016072 File Offset: 0x00014272
		[JsonProperty("application_id")]
		public Snowflake? ApplicationId { get; set; }

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0001607B File Offset: 0x0001427B
		// (set) Token: 0x060008F7 RID: 2295 RVA: 0x00016083 File Offset: 0x00014283
		[JsonProperty("parent_id")]
		public Snowflake? ParentId { get; set; }

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060008F8 RID: 2296 RVA: 0x0001608C File Offset: 0x0001428C
		// (set) Token: 0x060008F9 RID: 2297 RVA: 0x00016094 File Offset: 0x00014294
		[JsonProperty("last_pin_timestamp")]
		public DateTime? LastPinTimestamp { get; set; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0001609D File Offset: 0x0001429D
		// (set) Token: 0x060008FB RID: 2299 RVA: 0x000160A5 File Offset: 0x000142A5
		[JsonProperty("rtc_region")]
		public string RtcRegion { get; set; }

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x000160AE File Offset: 0x000142AE
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x000160B6 File Offset: 0x000142B6
		[JsonProperty("video_quality_mode")]
		public VideoQualityMode? VideoQualityMode { get; set; }

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x000160BF File Offset: 0x000142BF
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x000160C7 File Offset: 0x000142C7
		[JsonProperty("message_count")]
		public int? MessageCount { get; set; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x000160D0 File Offset: 0x000142D0
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x000160D8 File Offset: 0x000142D8
		[JsonProperty("member_count")]
		public int? MemberCount { get; set; }

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x000160E1 File Offset: 0x000142E1
		// (set) Token: 0x06000903 RID: 2307 RVA: 0x000160E9 File Offset: 0x000142E9
		[JsonProperty("thread_metadata")]
		public ThreadMetadata ThreadMetadata { get; set; }

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x000160F2 File Offset: 0x000142F2
		// (set) Token: 0x06000905 RID: 2309 RVA: 0x000160FA File Offset: 0x000142FA
		[JsonProperty("member")]
		public ThreadMember Member { get; set; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000906 RID: 2310 RVA: 0x00016103 File Offset: 0x00014303
		// (set) Token: 0x06000907 RID: 2311 RVA: 0x0001610B File Offset: 0x0001430B
		[JsonProperty("default_auto_archive_duration")]
		public int? DefaultAutoArchiveDuration { get; set; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x00016114 File Offset: 0x00014314
		// (set) Token: 0x06000909 RID: 2313 RVA: 0x0001611C File Offset: 0x0001431C
		[JsonProperty("permissions")]
		public string Permissions { get; set; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x00016128 File Offset: 0x00014328
		public Hash<Snowflake, ThreadMember> ThreadMembers
		{
			get
			{
				bool flag = this._threadMembers != null;
				Hash<Snowflake, ThreadMember> result;
				if (flag)
				{
					result = this._threadMembers;
				}
				else
				{
					bool flag2 = this.Type != ChannelType.GuildPublicThread && this.Type != ChannelType.GuildPrivateThread;
					if (flag2)
					{
						throw new InvalidChannelException("Trying to access ThreadMembers on channel that is not a thread");
					}
					result = (this._threadMembers = new Hash<Snowflake, ThreadMember>());
				}
				return result;
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x0600090B RID: 2315 RVA: 0x00016189 File Offset: 0x00014389
		public string Mention
		{
			get
			{
				return DiscordFormatting.MentionChannel(this.Id);
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x0600090C RID: 2316 RVA: 0x00016196 File Offset: 0x00014396
		public string IconUrl
		{
			get
			{
				return (!string.IsNullOrEmpty(this.Icon)) ? DiscordCdn.GetChannelIcon(this.Id, this.Icon) : null;
			}
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x000161BC File Offset: 0x000143BC
		public static void CreateGuildChannel(DiscordClient client, Snowflake guildId, ChannelCreate channel, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/guilds/{0}/channels", guildId), RequestMethod.POST, channel, callback, error);
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0001620C File Offset: 0x0001440C
		public static void GetChannel(DiscordClient client, Snowflake channelId, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}", channelId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00016258 File Offset: 0x00014458
		public void ModifyGroupDmChannel(DiscordClient client, GroupDmChannelUpdate update, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}", this.Id), RequestMethod.PATCH, update, callback, error);
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00016286 File Offset: 0x00014486
		public void ModifyGuildChannel(DiscordClient client, GuildChannelUpdate update, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}", this.Id), RequestMethod.PATCH, update, callback, error);
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x000162B4 File Offset: 0x000144B4
		public void ModifyThreadChannel(DiscordClient client, ThreadChannelUpdate update, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}", this.Id), RequestMethod.PATCH, update, callback, error);
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x000162E2 File Offset: 0x000144E2
		public void DeleteChannel(DiscordClient client, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}", this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x0001630F File Offset: 0x0001450F
		public void GetChannelMessages(DiscordClient client, ChannelMessagesRequest request = null, Action<List<DiscordMessage>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordMessage>>(string.Format("/channels/{0}/messages{1}", this.Id, (request != null) ? request.ToQueryString() : null), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0001634C File Offset: 0x0001454C
		public void GetChannelMessage(DiscordClient client, Snowflake messageId, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages/{1}", this.Id, messageId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x000163A4 File Offset: 0x000145A4
		public void CreateMessage(DiscordClient client, MessageCreate message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			message.Validate();
			message.ValidateChannelMessage();
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/channels/{0}/messages", this.Id), RequestMethod.POST, message, callback, error);
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x000163E0 File Offset: 0x000145E0
		public void CreateMessage(DiscordClient client, string message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			MessageCreate message2 = new MessageCreate
			{
				Content = message
			};
			this.CreateMessage(client, message2, callback, error);
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00016408 File Offset: 0x00014608
		public void CreateMessage(DiscordClient client, DiscordEmbed embed, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			MessageCreate message = new MessageCreate
			{
				Embeds = new List<DiscordEmbed>
				{
					embed
				}
			};
			this.CreateMessage(client, message, callback, error);
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x0001643C File Offset: 0x0001463C
		public void CreateMessage(DiscordClient client, List<DiscordEmbed> embeds, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			MessageCreate message = new MessageCreate
			{
				Embeds = embeds
			};
			this.CreateMessage(client, message, callback, error);
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x00016464 File Offset: 0x00014664
		public void BulkDeleteMessages(DiscordClient client, ICollection<Snowflake> messageIds, Action callback = null, Action<RestError> error = null)
		{
			bool flag = messageIds.Count < 2;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("messageIds", "Cannot delete less than 2 messages");
			}
			bool flag2 = messageIds.Count > 100;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("messageIds", "Cannot delete more than 100 messages");
			}
			Dictionary<string, ICollection<Snowflake>> dictionary = new Dictionary<string, ICollection<Snowflake>>();
			dictionary["messages"] = messageIds;
			Dictionary<string, ICollection<Snowflake>> data = dictionary;
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/messages/bulk-delete", this.Id), RequestMethod.POST, data, callback, error);
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x000164ED File Offset: 0x000146ED
		public void EditChannelPermissions(DiscordClient client, Overwrite overwrite, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/permissions/{1}", this.Id, overwrite.Id), RequestMethod.PUT, overwrite, callback, error);
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00016528 File Offset: 0x00014728
		public void EditChannelPermissions(DiscordClient client, Snowflake overwriteId, PermissionFlags? allow, PermissionFlags? deny, PermissionType type, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !overwriteId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("overwriteId");
			}
			Overwrite overwrite = new Overwrite
			{
				Id = overwriteId,
				Type = type,
				Allow = allow,
				Deny = deny
			};
			this.EditChannelPermissions(client, overwrite, callback, error);
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00016584 File Offset: 0x00014784
		public void GetChannelInvites(DiscordClient client, Action<List<DiscordInvite>> callback = null, Action<RestError> error = null)
		{
			bool flag = this.Type == ChannelType.Dm || this.Type == ChannelType.GroupDm;
			if (flag)
			{
				throw new InvalidChannelException("You can only get channel invites for guild channels.");
			}
			client.Bot.Rest.DoRequest<List<DiscordInvite>>(string.Format("/channels/{0}/invites", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x000165E1 File Offset: 0x000147E1
		public void CreateChannelInvite(DiscordClient client, ChannelInvite invite, Action<DiscordInvite> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordInvite>(string.Format("/channels/{0}/invites", this.Id), RequestMethod.POST, invite, callback, error);
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x0001660F File Offset: 0x0001480F
		public void DeleteChannelPermission(DiscordClient client, Overwrite overwrite, Action callback = null, Action<RestError> error = null)
		{
			this.DeleteChannelPermission(client, overwrite.Id, callback, error);
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x00016624 File Offset: 0x00014824
		public void DeleteChannelPermission(DiscordClient client, Snowflake overwriteId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !overwriteId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("overwriteId");
			}
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/permissions/{1}", this.Id, overwriteId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x0001667C File Offset: 0x0001487C
		public void FollowNewsChannel(DiscordClient client, Snowflake webhookChannelId, Action<FollowedChannel> callback = null, Action<RestError> error = null)
		{
			bool flag = !webhookChannelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("webhookChannelId");
			}
			client.Bot.Rest.DoRequest<FollowedChannel>(string.Format("/channels/{0}/followers?webhook_channel_id={1}", this.Id, webhookChannelId), RequestMethod.POST, null, callback, error);
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x000166D4 File Offset: 0x000148D4
		public void TriggerTypingIndicator(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/typing", this.Id), RequestMethod.POST, null, callback, error);
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x00016701 File Offset: 0x00014901
		public void GetPinnedMessages(DiscordClient client, Action<List<DiscordMessage>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<DiscordMessage>>(string.Format("/channels/{0}/pins", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x0001672E File Offset: 0x0001492E
		public void GroupDmAddRecipient(DiscordClient client, DiscordUser user, string accessToken, Action callback = null, Action<RestError> error = null)
		{
			this.GroupDmAddRecipient(client, user.Id, accessToken, user.Username, callback, error);
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x0001674C File Offset: 0x0001494C
		public void GroupDmAddRecipient(DiscordClient client, Snowflake userId, string accessToken, string nick, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["access_token"] = accessToken;
			dictionary["nick"] = nick;
			Dictionary<string, string> data = dictionary;
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/recipients/{1}", this.Id, userId), RequestMethod.PUT, data, callback, error);
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x000167C8 File Offset: 0x000149C8
		public void GroupDmRemoveRecipient(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/recipients/{1}", this.Id, userId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x00016820 File Offset: 0x00014A20
		public void StartThreadWithMessage(DiscordClient client, Snowflake messageId, ThreadCreate create, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}/messages/{1}/threads", this.Id, messageId), RequestMethod.POST, create, callback, error);
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x00016879 File Offset: 0x00014A79
		public void StartThreadWithoutMessage(DiscordClient client, ThreadCreate create, Action<DiscordChannel> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordChannel>(string.Format("/channels/{0}/threads", this.Id), RequestMethod.POST, create, callback, error);
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x000168A7 File Offset: 0x00014AA7
		public void JoinThread(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/thread-members/@me", this.Id), RequestMethod.PUT, null, callback, error);
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x000168D4 File Offset: 0x00014AD4
		public void AddThreadMember(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/thread-members/{1}", this.Id, userId), RequestMethod.PUT, null, callback, error);
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0001692C File Offset: 0x00014B2C
		public void LeaveThread(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/thread-members/@me", this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0001695C File Offset: 0x00014B5C
		public void RemoveThreadMember(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest(string.Format("/channels/{0}/thread-members/{1}", this.Id, userId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x000169B4 File Offset: 0x00014BB4
		public void GetThreadMember(DiscordClient client, Snowflake userId, Action<ThreadMember> callback = null, Action<RestError> error = null)
		{
			bool flag = !userId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("userId");
			}
			client.Bot.Rest.DoRequest<ThreadMember>(string.Format("/channels/{0}/thread-members/{1}", this.Id, userId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x00016A0C File Offset: 0x00014C0C
		public void ListThreadMembers(DiscordClient client, Action<List<ThreadMember>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<ThreadMember>>(string.Format("/channels/{0}/thread-members", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00016A39 File Offset: 0x00014C39
		[Obsolete("This route is deprecated and will be removed in v10. It is replaced by List Active Guild Threads.")]
		public void ListActiveThreads(DiscordClient client, Action<ThreadList> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<ThreadList>(string.Format("/channels/{0}/threads/active", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00016A66 File Offset: 0x00014C66
		public void ListPublicArchivedThreads(DiscordClient client, ThreadArchivedLookup lookup, Action<ThreadList> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<ThreadList>(string.Format("/channels/{0}/threads/archived/public{1}", this.Id, lookup.ToQueryString()), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x00016A9A File Offset: 0x00014C9A
		public void ListPrivateArchivedThreads(DiscordClient client, ThreadArchivedLookup lookup, Action<ThreadList> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<ThreadList>(string.Format("/channels/{0}/threads/archived/public{1}", this.Id, lookup.ToQueryString()), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x00016ACE File Offset: 0x00014CCE
		public void ListJoinedPrivateArchivedThreads(DiscordClient client, ThreadArchivedLookup lookup, Action<ThreadList> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<ThreadList>(string.Format("/channels/{0}/users/@me/threads/archived/private{1}", this.Id, lookup.ToQueryString()), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x00016B02 File Offset: 0x00014D02
		public void GetStageInstance(DiscordClient client, Action<StageInstance> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<StageInstance>(string.Format("/stage-instances/{0}", this.Id), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x00016B30 File Offset: 0x00014D30
		internal DiscordChannel Update(DiscordChannel channel)
		{
			DiscordChannel result = (DiscordChannel)base.MemberwiseClone();
			this.Type = channel.Type;
			bool flag = channel.Position != null;
			if (flag)
			{
				this.Position = channel.Position;
			}
			bool flag2 = channel.PermissionOverwrites != null;
			if (flag2)
			{
				this.PermissionOverwrites = channel.PermissionOverwrites;
			}
			bool flag3 = channel.Name != null;
			if (flag3)
			{
				this.Name = channel.Name;
			}
			bool flag4 = channel.Topic != null;
			if (flag4)
			{
				this.Topic = channel.Topic;
			}
			bool flag5 = channel.Nsfw != null;
			if (flag5)
			{
				this.Nsfw = channel.Nsfw;
			}
			bool flag6 = channel.Bitrate != null;
			if (flag6)
			{
				this.Bitrate = channel.Bitrate;
			}
			bool flag7 = channel.UserLimit != null;
			if (flag7)
			{
				this.UserLimit = channel.UserLimit;
			}
			bool flag8 = channel.RateLimitPerUser != null;
			if (flag8)
			{
				this.RateLimitPerUser = channel.RateLimitPerUser;
			}
			bool flag9 = channel.Icon != null;
			if (flag9)
			{
				this.Icon = channel.Icon;
			}
			bool flag10 = channel.OwnerId != null;
			if (flag10)
			{
				this.OwnerId = channel.OwnerId;
			}
			bool flag11 = channel.ApplicationId != null;
			if (flag11)
			{
				this.ApplicationId = channel.ApplicationId;
			}
			bool flag12 = channel.LastPinTimestamp != null;
			if (flag12)
			{
				this.LastPinTimestamp = channel.LastPinTimestamp;
			}
			bool flag13 = channel.VideoQualityMode != null;
			if (flag13)
			{
				this.VideoQualityMode = channel.VideoQualityMode;
			}
			bool flag14 = channel.MessageCount != null;
			if (flag14)
			{
				this.MessageCount = channel.MessageCount;
			}
			bool flag15 = channel.MemberCount != null;
			if (flag15)
			{
				this.MemberCount = channel.MemberCount;
			}
			bool flag16 = channel.ThreadMetadata != null;
			if (flag16)
			{
				this.ThreadMetadata = channel.ThreadMetadata;
			}
			bool flag17 = channel.Member != null;
			if (flag17)
			{
				this.Member = channel.Member;
			}
			bool flag18 = channel.DefaultAutoArchiveDuration != null;
			if (flag18)
			{
				this.DefaultAutoArchiveDuration = channel.DefaultAutoArchiveDuration;
			}
			bool flag19 = channel.Permissions != null;
			if (flag19)
			{
				this.Permissions = channel.Permissions;
			}
			this.ParentId = channel.ParentId;
			return result;
		}

		// Token: 0x04000551 RID: 1361
		private Hash<Snowflake, ThreadMember> _threadMembers;
	}
}
