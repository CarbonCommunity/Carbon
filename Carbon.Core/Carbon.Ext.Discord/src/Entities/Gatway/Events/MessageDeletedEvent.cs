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
	// Token: 0x020000E0 RID: 224
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageDeletedEvent
	{
		// Token: 0x170002AD RID: 685
		// (get) Token: 0x060007F8 RID: 2040 RVA: 0x000155C7 File Offset: 0x000137C7
		// (set) Token: 0x060007F9 RID: 2041 RVA: 0x000155CF File Offset: 0x000137CF
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x000155D8 File Offset: 0x000137D8
		// (set) Token: 0x060007FB RID: 2043 RVA: 0x000155E0 File Offset: 0x000137E0
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060007FC RID: 2044 RVA: 0x000155E9 File Offset: 0x000137E9
		// (set) Token: 0x060007FD RID: 2045 RVA: 0x000155F1 File Offset: 0x000137F1
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }
	}
}
