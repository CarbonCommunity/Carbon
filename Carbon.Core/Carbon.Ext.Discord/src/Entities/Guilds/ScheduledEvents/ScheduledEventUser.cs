/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
	// Token: 0x020000C1 RID: 193
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ScheduledEventUser
	{
		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x00014F07 File Offset: 0x00013107
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x00014F0F File Offset: 0x0001310F
		[JsonProperty("guild_scheduled_event_id")]
		public Snowflake GuildScheduledEventId { get; set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x00014F18 File Offset: 0x00013118
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x00014F20 File Offset: 0x00013120
		[JsonProperty("guild_scheduled_event_id")]
		public DiscordUser User { get; set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x00014F29 File Offset: 0x00013129
		// (set) Token: 0x06000744 RID: 1860 RVA: 0x00014F31 File Offset: 0x00013131
		[JsonProperty("member")]
		public GuildMember Member { get; set; }
	}
}
