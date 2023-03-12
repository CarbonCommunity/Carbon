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
	// Token: 0x020000C8 RID: 200
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ChannelPinsUpdatedEvent
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000765 RID: 1893 RVA: 0x00015197 File Offset: 0x00013397
		// (set) Token: 0x06000766 RID: 1894 RVA: 0x0001519F File Offset: 0x0001339F
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000767 RID: 1895 RVA: 0x000151A8 File Offset: 0x000133A8
		// (set) Token: 0x06000768 RID: 1896 RVA: 0x000151B0 File Offset: 0x000133B0
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000769 RID: 1897 RVA: 0x000151B9 File Offset: 0x000133B9
		// (set) Token: 0x0600076A RID: 1898 RVA: 0x000151C1 File Offset: 0x000133C1
		[JsonProperty("last_pin_timestamp")]
		public DateTime? LastPinTimestamp { get; set; }
	}
}
