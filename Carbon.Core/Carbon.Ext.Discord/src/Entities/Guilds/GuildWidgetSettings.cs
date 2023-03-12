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
	// Token: 0x020000B6 RID: 182
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildWidgetSettings
	{
		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x00014844 File Offset: 0x00012A44
		// (set) Token: 0x060006E2 RID: 1762 RVA: 0x0001484C File Offset: 0x00012A4C
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060006E3 RID: 1763 RVA: 0x00014855 File Offset: 0x00012A55
		// (set) Token: 0x060006E4 RID: 1764 RVA: 0x0001485D File Offset: 0x00012A5D
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }
	}
}
