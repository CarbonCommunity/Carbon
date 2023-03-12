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
	// Token: 0x020000EB RID: 235
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class WebhooksUpdatedEvent
	{
		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000855 RID: 2133 RVA: 0x00015892 File Offset: 0x00013A92
		// (set) Token: 0x06000856 RID: 2134 RVA: 0x0001589A File Offset: 0x00013A9A
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000857 RID: 2135 RVA: 0x000158A3 File Offset: 0x00013AA3
		// (set) Token: 0x06000858 RID: 2136 RVA: 0x000158AB File Offset: 0x00013AAB
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }
	}
}
