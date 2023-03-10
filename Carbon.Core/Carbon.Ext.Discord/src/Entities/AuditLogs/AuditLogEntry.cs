/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
	// Token: 0x0200010E RID: 270
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogEntry
	{
		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060009C1 RID: 2497 RVA: 0x00017462 File Offset: 0x00015662
		// (set) Token: 0x060009C2 RID: 2498 RVA: 0x0001746A File Offset: 0x0001566A
		[JsonProperty("target_id")]
		public Snowflake TargetId { get; set; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060009C3 RID: 2499 RVA: 0x00017473 File Offset: 0x00015673
		// (set) Token: 0x060009C4 RID: 2500 RVA: 0x0001747B File Offset: 0x0001567B
		[JsonProperty("changes")]
		public List<AuditLogChange> Changes { get; set; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x060009C5 RID: 2501 RVA: 0x00017484 File Offset: 0x00015684
		// (set) Token: 0x060009C6 RID: 2502 RVA: 0x0001748C File Offset: 0x0001568C
		[JsonProperty("user_id")]
		public Snowflake? UserId { get; set; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x060009C7 RID: 2503 RVA: 0x00017495 File Offset: 0x00015695
		// (set) Token: 0x060009C8 RID: 2504 RVA: 0x0001749D File Offset: 0x0001569D
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x060009C9 RID: 2505 RVA: 0x000174A6 File Offset: 0x000156A6
		// (set) Token: 0x060009CA RID: 2506 RVA: 0x000174AE File Offset: 0x000156AE
		[JsonProperty("action_type")]
		public AuditLogActionType? ActionType { get; set; }

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x060009CB RID: 2507 RVA: 0x000174B7 File Offset: 0x000156B7
		// (set) Token: 0x060009CC RID: 2508 RVA: 0x000174BF File Offset: 0x000156BF
		[JsonProperty("options")]
		public OptionalAuditEntryInfo Options { get; set; }

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060009CD RID: 2509 RVA: 0x000174C8 File Offset: 0x000156C8
		// (set) Token: 0x060009CE RID: 2510 RVA: 0x000174D0 File Offset: 0x000156D0
		[JsonProperty("reason")]
		public string Reason { get; set; }
	}
}
