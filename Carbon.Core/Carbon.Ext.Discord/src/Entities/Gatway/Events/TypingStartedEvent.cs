/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E9 RID: 233
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class TypingStartedEvent
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000843 RID: 2115 RVA: 0x0001580A File Offset: 0x00013A0A
		// (set) Token: 0x06000844 RID: 2116 RVA: 0x00015812 File Offset: 0x00013A12
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x0001581B File Offset: 0x00013A1B
		// (set) Token: 0x06000846 RID: 2118 RVA: 0x00015823 File Offset: 0x00013A23
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000847 RID: 2119 RVA: 0x0001582C File Offset: 0x00013A2C
		// (set) Token: 0x06000848 RID: 2120 RVA: 0x00015834 File Offset: 0x00013A34
		[JsonProperty("user_id")]
		public Snowflake UserId { get; set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x0001583D File Offset: 0x00013A3D
		// (set) Token: 0x0600084A RID: 2122 RVA: 0x00015845 File Offset: 0x00013A45
		[JsonProperty("timestamp")]
		public int? Timestamp { get; set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600084B RID: 2123 RVA: 0x0001584E File Offset: 0x00013A4E
		// (set) Token: 0x0600084C RID: 2124 RVA: 0x00015856 File Offset: 0x00013A56
		[JsonProperty("member")]
		public GuildMember Member { get; set; }
	}
}
