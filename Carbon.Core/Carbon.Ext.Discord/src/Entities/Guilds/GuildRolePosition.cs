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
	// Token: 0x020000B1 RID: 177
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildRolePosition
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00014756 File Offset: 0x00012956
		// (set) Token: 0x060006C2 RID: 1730 RVA: 0x0001475E File Offset: 0x0001295E
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00014767 File Offset: 0x00012967
		// (set) Token: 0x060006C4 RID: 1732 RVA: 0x0001476F File Offset: 0x0001296F
		[JsonProperty("position")]
		public int Position { get; set; }
	}
}
