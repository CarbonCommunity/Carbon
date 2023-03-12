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
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
	// Token: 0x02000042 RID: 66
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordWebhook
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000DE67 File Offset: 0x0000C067
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x0000DE6F File Offset: 0x0000C06F
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000DE78 File Offset: 0x0000C078
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x0000DE80 File Offset: 0x0000C080
		[JsonProperty("type")]
		public WebhookType Type { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000DE89 File Offset: 0x0000C089
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000DE91 File Offset: 0x0000C091
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000DE9A File Offset: 0x0000C09A
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000DEA2 File Offset: 0x0000C0A2
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000DEAB File Offset: 0x0000C0AB
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000DEB3 File Offset: 0x0000C0B3
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0000DEBC File Offset: 0x0000C0BC
		// (set) Token: 0x060001DD RID: 477 RVA: 0x0000DEC4 File Offset: 0x0000C0C4
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000DECD File Offset: 0x0000C0CD
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000DED5 File Offset: 0x0000C0D5
		[JsonProperty("avatar")]
		public string Avatar { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000DEDE File Offset: 0x0000C0DE
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000DEE6 File Offset: 0x0000C0E6
		[JsonProperty("token")]
		public string Token { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000DEEF File Offset: 0x0000C0EF
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x0000DEF7 File Offset: 0x0000C0F7
		[JsonProperty("application_id")]
		public Snowflake ApplicationId { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000DF00 File Offset: 0x0000C100
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000DF08 File Offset: 0x0000C108
		[JsonProperty("source_guild")]
		public DiscordGuild SourceGuild { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000DF11 File Offset: 0x0000C111
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000DF19 File Offset: 0x0000C119
		[JsonProperty("source_channel")]
		public Snowflake SourceChannel { get; set; }

		// Token: 0x060001E8 RID: 488 RVA: 0x0000DF24 File Offset: 0x0000C124
		public static void CreateWebhook(DiscordClient client, Snowflake channelId, string name, string avatar = null, Action<DiscordWebhook> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["name"] = name;
			dictionary["avatar"] = avatar;
			Dictionary<string, string> data = dictionary;
			client.Bot.Rest.DoRequest<DiscordWebhook>(string.Format("/channels/{0}/webhooks", channelId), RequestMethod.POST, data, callback, error);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000DF94 File Offset: 0x0000C194
		public static void GetChannelWebhooks(DiscordClient client, Snowflake channelId, Action<List<DiscordWebhook>> callback = null, Action<RestError> error = null)
		{
			bool flag = !channelId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			client.Bot.Rest.DoRequest<List<DiscordWebhook>>(string.Format("/channels/{0}/webhooks", channelId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000DFE0 File Offset: 0x0000C1E0
		public static void GetGuildWebhooks(DiscordClient client, Snowflake guildId, Action<List<DiscordWebhook>> callback = null, Action<RestError> error = null)
		{
			bool flag = !guildId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("guildId");
			}
			client.Bot.Rest.DoRequest<List<DiscordWebhook>>(string.Format("/guilds/{0}/webhooks", guildId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000E02C File Offset: 0x0000C22C
		public static void GetWebhook(DiscordClient client, Snowflake webhookId, Action<DiscordWebhook> callback = null, Action<RestError> error = null)
		{
			bool flag = !webhookId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("webhookId");
			}
			client.Bot.Rest.DoRequest<DiscordWebhook>(string.Format("/webhooks/{0}", webhookId), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000E078 File Offset: 0x0000C278
		public static void GetWebhookWithToken(DiscordClient client, Snowflake webhookId, string webhookToken, Action<DiscordWebhook> callback = null, Action<RestError> error = null)
		{
			bool flag = !webhookId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("webhookId");
			}
			client.Bot.Rest.DoRequest<DiscordWebhook>(string.Format("/webhooks/{0}/{1}", webhookId, webhookToken), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000E0C8 File Offset: 0x0000C2C8
		public static void GetWebhookWithUrl(DiscordClient client, string webhookUrl, Action<DiscordWebhook> callback = null, Action<RestError> error = null)
		{
			string[] array = webhookUrl.Split(new char[]
			{
				'/'
			});
			string str = array[array.Length - 2];
			string str2 = array[array.Length - 1];
			client.Bot.Rest.DoRequest<DiscordWebhook>("/webhooks/" + str + "/" + str2, RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000E120 File Offset: 0x0000C320
		public void ModifyWebhook(DiscordClient client, string name = null, string avatar = null, Snowflake? channelId = null, Action<DiscordWebhook> callback = null, Action<RestError> error = null)
		{
			bool flag = channelId != null && !channelId.Value.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("channelId");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["name"] = name;
			dictionary["avatar"] = avatar;
			dictionary["channel_id"] = channelId;
			Dictionary<string, object> data = dictionary;
			client.Bot.Rest.DoRequest<DiscordWebhook>(string.Format("/webhooks/{0}", this.Id), RequestMethod.PATCH, data, callback, error);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000E1BC File Offset: 0x0000C3BC
		public void ModifyWebhookWithToken(DiscordClient client, string name = null, string avatar = null, Action<DiscordWebhook> callback = null, Action<RestError> error = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["name"] = name;
			dictionary["avatar"] = avatar;
			Dictionary<string, object> data = dictionary;
			client.Bot.Rest.DoRequest<DiscordWebhook>(string.Format("/webhooks/{0}/{1}", this.Id, this.Token), RequestMethod.PATCH, data, callback, error);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000E21C File Offset: 0x0000C41C
		public void DeleteWebhook(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/webhooks/{0}", this.Id), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000E249 File Offset: 0x0000C449
		public void DeleteWebhookWithToken(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/webhooks/{0}/{1}", this.Id, this.Token), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000E27C File Offset: 0x0000C47C
		public void ExecuteWebhook(DiscordClient client, WebhookCreateMessage message, WebhookExecuteParams executeParams = null, Action callback = null, Action<RestError> error = null)
		{
			bool flag = executeParams == null;
			if (flag)
			{
				executeParams = new WebhookExecuteParams();
			}
			message.Validate();
			message.ValidateWebhookMessage();
			client.Bot.Rest.DoRequest(string.Format("/webhooks/{0}/{1}{2}{3}", new object[]
			{
				this.Id,
				this.Token,
				executeParams.GetWebhookFormat(),
				executeParams.ToQueryString()
			}), RequestMethod.POST, message, callback, error);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000E2FC File Offset: 0x0000C4FC
		public void ExecuteWebhook(DiscordClient client, WebhookCreateMessage message, WebhookExecuteParams executeParams = null, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = executeParams == null;
			if (flag)
			{
				executeParams = new WebhookExecuteParams();
			}
			executeParams.Wait = true;
			message.Validate();
			message.ValidateWebhookMessage();
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/webhooks/{0}/{1}{2}{3}", new object[]
			{
				this.Id,
				this.Token,
				executeParams.GetWebhookFormat(),
				executeParams.ToQueryString()
			}), RequestMethod.POST, message, callback, error);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000E384 File Offset: 0x0000C584
		public void GetWebhookMessage(DiscordClient client, Snowflake messageId, WebhookMessageParams messageParams = null, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			bool flag2 = messageParams == null;
			if (flag2)
			{
				messageParams = new WebhookMessageParams();
			}
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/webhooks/{0}/{1}/messages/{2}{3}", new object[]
			{
				this.Id,
				this.Token,
				messageId,
				messageParams.ToQueryString()
			}), RequestMethod.GET, null, callback, error);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000E40C File Offset: 0x0000C60C
		public void EditWebhookMessage(DiscordClient client, Snowflake messageId, DiscordMessage message, WebhookMessageParams messageParams = null, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = messageParams == null;
			if (flag)
			{
				messageParams = new WebhookMessageParams();
			}
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/webhooks/{0}/{1}/messages/{2}{3}", new object[]
			{
				this.Id,
				this.Token,
				messageId,
				messageParams.ToQueryString()
			}), RequestMethod.PATCH, message, callback, error);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000E480 File Offset: 0x0000C680
		public void DeleteWebhookMessage(DiscordClient client, Snowflake messageId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest(string.Format("/webhooks/{0}/{1}/messages/{2}", this.Id, this.Token, messageId), RequestMethod.DELETE, null, callback, error);
		}
	}
}
