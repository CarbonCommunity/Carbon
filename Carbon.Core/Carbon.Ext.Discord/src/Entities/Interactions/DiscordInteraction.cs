/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Webhooks;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x0200007A RID: 122
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordInteraction
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x000111E8 File Offset: 0x0000F3E8
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x000111F0 File Offset: 0x0000F3F0
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x000111F9 File Offset: 0x0000F3F9
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x00011201 File Offset: 0x0000F401
		[JsonProperty("application_id")]
		public Snowflake ApplicationId { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000458 RID: 1112 RVA: 0x0001120A File Offset: 0x0000F40A
		// (set) Token: 0x06000459 RID: 1113 RVA: 0x00011212 File Offset: 0x0000F412
		[JsonProperty("type")]
		public InteractionType Type { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x0001121B File Offset: 0x0000F41B
		// (set) Token: 0x0600045B RID: 1115 RVA: 0x00011223 File Offset: 0x0000F423
		[JsonProperty("data")]
		public InteractionData Data { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x0001122C File Offset: 0x0000F42C
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x00011234 File Offset: 0x0000F434
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x0001123D File Offset: 0x0000F43D
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x00011245 File Offset: 0x0000F445
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x0001124E File Offset: 0x0000F44E
		// (set) Token: 0x06000461 RID: 1121 RVA: 0x00011256 File Offset: 0x0000F456
		[JsonProperty("member")]
		public GuildMember Member { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x0001125F File Offset: 0x0000F45F
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x00011267 File Offset: 0x0000F467
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00011270 File Offset: 0x0000F470
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x00011278 File Offset: 0x0000F478
		[JsonProperty("token")]
		public string Token { get; set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00011281 File Offset: 0x0000F481
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x00011289 File Offset: 0x0000F489
		[JsonProperty("version")]
		public int Version { get; set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00011292 File Offset: 0x0000F492
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x0001129A File Offset: 0x0000F49A
		[JsonProperty("message")]
		public DiscordMessage Message { get; set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x000112A3 File Offset: 0x0000F4A3
		// (set) Token: 0x0600046B RID: 1131 RVA: 0x000112AB File Offset: 0x0000F4AB
		[JsonProperty("locale")]
		public string Locale { get; set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600046C RID: 1132 RVA: 0x000112B4 File Offset: 0x0000F4B4
		// (set) Token: 0x0600046D RID: 1133 RVA: 0x000112BC File Offset: 0x0000F4BC
		[JsonProperty("guild_locale")]
		public string GuildLocale { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x000112C8 File Offset: 0x0000F4C8
		public InteractionDataParsed Parsed
		{
			get
			{
				InteractionDataParsed result;
				if ((result = this._parsed) == null)
				{
					result = (this._parsed = new InteractionDataParsed(this));
				}
				return result;
			}
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x000112F0 File Offset: 0x0000F4F0
		public void CreateInteractionResponse(DiscordClient client, InteractionResponse response, Action callback = null, Action<RestError> error = null)
		{
			InteractionCallbackData data = response.Data;
			if (data != null)
			{
				data.Validate();
			}
			client.Bot.Rest.DoRequest(string.Format("/interactions/{0}/{1}/callback", this.Id, this.Token), RequestMethod.POST, response, callback, error);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00011341 File Offset: 0x0000F541
		public void EditOriginalInteractionResponse(DiscordClient client, DiscordMessage message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/webhooks/{0}/{1}/messages/@original", this.ApplicationId, this.Token), RequestMethod.PATCH, message, callback, error);
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00011375 File Offset: 0x0000F575
		public void DeleteOriginalInteractionResponse(DiscordClient client, Action callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest(string.Format("/webhooks/{0}/{1}/messages/@original", this.ApplicationId, this.Token), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x000113A8 File Offset: 0x0000F5A8
		public void CreateFollowUpMessage(DiscordClient client, WebhookCreateMessage message, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			message.Validate();
			message.ValidateInteractionMessage();
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/webhooks/{0}/{1}", this.ApplicationId, this.Token), RequestMethod.POST, message, callback, error);
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x000113F8 File Offset: 0x0000F5F8
		public void EditFollowUpMessage(DiscordClient client, Snowflake messageId, CommandFollowupUpdate edit, Action<DiscordMessage> callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest<DiscordMessage>(string.Format("/webhooks/{0}/{1}/messages/{2}", this.ApplicationId, this.Token, messageId), RequestMethod.PATCH, edit, callback, error);
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00011458 File Offset: 0x0000F658
		public void DeleteFollowUpMessage(DiscordClient client, Snowflake messageId, Action callback = null, Action<RestError> error = null)
		{
			bool flag = !messageId.IsValid();
			if (flag)
			{
				throw new InvalidSnowflakeException("messageId");
			}
			client.Bot.Rest.DoRequest(string.Format("/webhooks/{0}/{1}/messages/{2}", this.ApplicationId, this.Token, messageId), RequestMethod.DELETE, null, callback, error);
		}

		// Token: 0x040002B5 RID: 693
		private InteractionDataParsed _parsed;
	}
}
