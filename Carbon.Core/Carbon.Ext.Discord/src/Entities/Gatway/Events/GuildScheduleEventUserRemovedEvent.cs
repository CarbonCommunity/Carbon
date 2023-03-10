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
	// Token: 0x020000D8 RID: 216
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildScheduleEventUserRemovedEvent
	{
		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x000153F3 File Offset: 0x000135F3
		// (set) Token: 0x060007BB RID: 1979 RVA: 0x000153FB File Offset: 0x000135FB
		[JsonProperty("guild_scheduled_event_id")]
		public Snowflake GuildScheduledEventId { get; set; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060007BC RID: 1980 RVA: 0x00015404 File Offset: 0x00013604
		// (set) Token: 0x060007BD RID: 1981 RVA: 0x0001540C File Offset: 0x0001360C
		[JsonProperty("user_id")]
		public Snowflake UserId { get; set; }

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060007BE RID: 1982 RVA: 0x00015415 File Offset: 0x00013615
		// (set) Token: 0x060007BF RID: 1983 RVA: 0x0001541D File Offset: 0x0001361D
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
