/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000F9 RID: 249
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ChannelMention : ISnowflakeEntity
	{
		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x00015DCB File Offset: 0x00013FCB
		// (set) Token: 0x060008C4 RID: 2244 RVA: 0x00015DD3 File Offset: 0x00013FD3
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060008C5 RID: 2245 RVA: 0x00015DDC File Offset: 0x00013FDC
		// (set) Token: 0x060008C6 RID: 2246 RVA: 0x00015DE4 File Offset: 0x00013FE4
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060008C7 RID: 2247 RVA: 0x00015DED File Offset: 0x00013FED
		// (set) Token: 0x060008C8 RID: 2248 RVA: 0x00015DF5 File Offset: 0x00013FF5
		[JsonProperty("type")]
		public ChannelType Type { get; set; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060008C9 RID: 2249 RVA: 0x00015DFE File Offset: 0x00013FFE
		// (set) Token: 0x060008CA RID: 2250 RVA: 0x00015E06 File Offset: 0x00014006
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
