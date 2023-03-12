/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A5 RID: 165
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildChannelPosition
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0001417F File Offset: 0x0001237F
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x00014187 File Offset: 0x00012387
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x00014190 File Offset: 0x00012390
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x00014198 File Offset: 0x00012398
		[JsonProperty("position")]
		public int Position { get; set; }
	}
}
