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
	// Token: 0x020000D7 RID: 215
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildScheduleEventUserAddedEvent
	{
		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x000153C0 File Offset: 0x000135C0
		// (set) Token: 0x060007B4 RID: 1972 RVA: 0x000153C8 File Offset: 0x000135C8
		[JsonProperty("guild_scheduled_event_id")]
		public Snowflake GuildScheduledEventId { get; set; }

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x000153D1 File Offset: 0x000135D1
		// (set) Token: 0x060007B6 RID: 1974 RVA: 0x000153D9 File Offset: 0x000135D9
		[JsonProperty("user_id")]
		public Snowflake UserId { get; set; }

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x000153E2 File Offset: 0x000135E2
		// (set) Token: 0x060007B8 RID: 1976 RVA: 0x000153EA File Offset: 0x000135EA
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
