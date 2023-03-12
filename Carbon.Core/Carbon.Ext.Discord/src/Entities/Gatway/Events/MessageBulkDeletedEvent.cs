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

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000DF RID: 223
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageBulkDeletedEvent
	{
		// Token: 0x170002AA RID: 682
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x00015594 File Offset: 0x00013794
		// (set) Token: 0x060007F2 RID: 2034 RVA: 0x0001559C File Offset: 0x0001379C
		[JsonProperty("ids")]
		public List<Snowflake> Ids { get; set; }

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060007F3 RID: 2035 RVA: 0x000155A5 File Offset: 0x000137A5
		// (set) Token: 0x060007F4 RID: 2036 RVA: 0x000155AD File Offset: 0x000137AD
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060007F5 RID: 2037 RVA: 0x000155B6 File Offset: 0x000137B6
		// (set) Token: 0x060007F6 RID: 2038 RVA: 0x000155BE File Offset: 0x000137BE
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }
	}
}
