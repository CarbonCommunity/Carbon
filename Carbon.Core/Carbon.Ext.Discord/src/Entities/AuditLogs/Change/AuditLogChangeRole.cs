/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
	// Token: 0x02000115 RID: 277
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeRole
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x000177D6 File Offset: 0x000159D6
		// (set) Token: 0x06000A31 RID: 2609 RVA: 0x000177DE File Offset: 0x000159DE
		[JsonProperty("permissions")]
		public string Permissions { get; set; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000A32 RID: 2610 RVA: 0x000177E7 File Offset: 0x000159E7
		// (set) Token: 0x06000A33 RID: 2611 RVA: 0x000177EF File Offset: 0x000159EF
		[JsonProperty("color")]
		public DiscordColor Color { get; set; }

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000A34 RID: 2612 RVA: 0x000177F8 File Offset: 0x000159F8
		// (set) Token: 0x06000A35 RID: 2613 RVA: 0x00017800 File Offset: 0x00015A00
		[JsonProperty("hoist")]
		public bool Hoist { get; set; }

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000A36 RID: 2614 RVA: 0x00017809 File Offset: 0x00015A09
		// (set) Token: 0x06000A37 RID: 2615 RVA: 0x00017811 File Offset: 0x00015A11
		[JsonProperty("mentionable")]
		public bool Mentionable { get; set; }

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06000A38 RID: 2616 RVA: 0x0001781A File Offset: 0x00015A1A
		// (set) Token: 0x06000A39 RID: 2617 RVA: 0x00017822 File Offset: 0x00015A22
		[JsonProperty("allow")]
		public int? Allow { get; set; }

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000A3A RID: 2618 RVA: 0x0001782B File Offset: 0x00015A2B
		// (set) Token: 0x06000A3B RID: 2619 RVA: 0x00017833 File Offset: 0x00015A33
		[JsonProperty("deny")]
		public int? Deny { get; set; }
	}
}
