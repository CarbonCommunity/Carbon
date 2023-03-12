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
	// Token: 0x020000E3 RID: 227
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageReactionRemovedAllEvent
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x0600080F RID: 2063 RVA: 0x0001567A File Offset: 0x0001387A
		// (set) Token: 0x06000810 RID: 2064 RVA: 0x00015682 File Offset: 0x00013882
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x0001568B File Offset: 0x0001388B
		// (set) Token: 0x06000812 RID: 2066 RVA: 0x00015693 File Offset: 0x00013893
		[JsonProperty("message_id")]
		public Snowflake MessageId { get; set; }

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000813 RID: 2067 RVA: 0x0001569C File Offset: 0x0001389C
		// (set) Token: 0x06000814 RID: 2068 RVA: 0x000156A4 File Offset: 0x000138A4
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }
	}
}
