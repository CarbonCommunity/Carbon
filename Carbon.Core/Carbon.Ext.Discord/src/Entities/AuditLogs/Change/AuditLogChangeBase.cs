/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
	// Token: 0x02000110 RID: 272
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeBase
	{
		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x00017561 File Offset: 0x00015761
		// (set) Token: 0x060009E2 RID: 2530 RVA: 0x00017569 File Offset: 0x00015769
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x060009E3 RID: 2531 RVA: 0x00017572 File Offset: 0x00015772
		// (set) Token: 0x060009E4 RID: 2532 RVA: 0x0001757A File Offset: 0x0001577A
		[JsonProperty("type")]
		public int? Type { get; set; }
	}
}
