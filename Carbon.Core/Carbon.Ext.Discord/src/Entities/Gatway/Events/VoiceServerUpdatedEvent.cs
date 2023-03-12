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
	// Token: 0x020000EA RID: 234
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class VoiceServerUpdatedEvent
	{
		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600084E RID: 2126 RVA: 0x0001585F File Offset: 0x00013A5F
		// (set) Token: 0x0600084F RID: 2127 RVA: 0x00015867 File Offset: 0x00013A67
		[JsonProperty("token")]
		public string Token { get; set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000850 RID: 2128 RVA: 0x00015870 File Offset: 0x00013A70
		// (set) Token: 0x06000851 RID: 2129 RVA: 0x00015878 File Offset: 0x00013A78
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000852 RID: 2130 RVA: 0x00015881 File Offset: 0x00013A81
		// (set) Token: 0x06000853 RID: 2131 RVA: 0x00015889 File Offset: 0x00013A89
		[JsonProperty("endpoint")]
		public string Endpoint { get; set; }
	}
}
