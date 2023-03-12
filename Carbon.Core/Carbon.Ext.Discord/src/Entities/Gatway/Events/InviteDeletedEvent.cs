/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000DE RID: 222
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InviteDeletedEvent
	{
		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x00015561 File Offset: 0x00013761
		// (set) Token: 0x060007EB RID: 2027 RVA: 0x00015569 File Offset: 0x00013769
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060007EC RID: 2028 RVA: 0x00015572 File Offset: 0x00013772
		// (set) Token: 0x060007ED RID: 2029 RVA: 0x0001557A File Offset: 0x0001377A
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060007EE RID: 2030 RVA: 0x00015583 File Offset: 0x00013783
		// (set) Token: 0x060007EF RID: 2031 RVA: 0x0001558B File Offset: 0x0001378B
		[JsonProperty("code")]
		public string Code { get; set; }
	}
}
