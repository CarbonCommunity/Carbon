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

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000D0 RID: 208
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMemberBannedEvent
	{
		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x0001528E File Offset: 0x0001348E
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x00015296 File Offset: 0x00013496
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x0001529F File Offset: 0x0001349F
		// (set) Token: 0x0600078B RID: 1931 RVA: 0x000152A7 File Offset: 0x000134A7
		[JsonProperty("user")]
		public DiscordUser User { get; set; }
	}
}
