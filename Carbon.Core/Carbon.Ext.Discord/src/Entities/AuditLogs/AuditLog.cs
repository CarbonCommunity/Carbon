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
using Oxide.Ext.Discord.Entities.Integrations;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Webhooks;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
	// Token: 0x0200010B RID: 267
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLog
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060009AF RID: 2479 RVA: 0x000173B2 File Offset: 0x000155B2
		// (set) Token: 0x060009B0 RID: 2480 RVA: 0x000173BA File Offset: 0x000155BA
		[JsonProperty("webhooks")]
		public List<DiscordWebhook> Webhooks { get; set; }

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060009B1 RID: 2481 RVA: 0x000173C3 File Offset: 0x000155C3
		// (set) Token: 0x060009B2 RID: 2482 RVA: 0x000173CB File Offset: 0x000155CB
		[JsonProperty("users")]
		public List<DiscordUser> Users { get; set; }

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x000173D4 File Offset: 0x000155D4
		// (set) Token: 0x060009B4 RID: 2484 RVA: 0x000173DC File Offset: 0x000155DC
		[JsonProperty("audit_log_entries")]
		public List<AuditLogEntry> AuditLogEntries { get; set; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060009B5 RID: 2485 RVA: 0x000173E5 File Offset: 0x000155E5
		// (set) Token: 0x060009B6 RID: 2486 RVA: 0x000173ED File Offset: 0x000155ED
		[JsonProperty("integrations")]
		public List<Integration> Integrations { get; set; }

		// Token: 0x060009B7 RID: 2487 RVA: 0x000173F6 File Offset: 0x000155F6
		public static void GetGuildAuditLog(DiscordClient client, DiscordGuild guild, Action<AuditLog> callback = null)
		{
			AuditLog.GetGuildAuditLog(client, guild.Id, callback, null);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00017407 File Offset: 0x00015607
		public static void GetGuildAuditLog(DiscordClient client, Snowflake guildId, Action<AuditLog> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<AuditLog>(string.Format("/guilds/{0}/audit-logs", guildId), RequestMethod.GET, null, callback, error);
		}
	}
}
