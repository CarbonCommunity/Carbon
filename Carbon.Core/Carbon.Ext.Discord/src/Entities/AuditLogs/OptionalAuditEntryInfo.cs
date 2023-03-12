/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
	// Token: 0x0200010F RID: 271
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class OptionalAuditEntryInfo
	{
		// Token: 0x1700036D RID: 877
		// (get) Token: 0x060009D0 RID: 2512 RVA: 0x000174D9 File Offset: 0x000156D9
		// (set) Token: 0x060009D1 RID: 2513 RVA: 0x000174E1 File Offset: 0x000156E1
		[JsonProperty("delete_member_days")]
		public string DeleteMemberDays { get; set; }

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060009D2 RID: 2514 RVA: 0x000174EA File Offset: 0x000156EA
		// (set) Token: 0x060009D3 RID: 2515 RVA: 0x000174F2 File Offset: 0x000156F2
		[JsonProperty("members_removed")]
		public string MembersRemoved { get; set; }

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060009D4 RID: 2516 RVA: 0x000174FB File Offset: 0x000156FB
		// (set) Token: 0x060009D5 RID: 2517 RVA: 0x00017503 File Offset: 0x00015703
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060009D6 RID: 2518 RVA: 0x0001750C File Offset: 0x0001570C
		// (set) Token: 0x060009D7 RID: 2519 RVA: 0x00017514 File Offset: 0x00015714
		[JsonProperty("message_id")]
		public Snowflake MessageId { get; set; }

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060009D8 RID: 2520 RVA: 0x0001751D File Offset: 0x0001571D
		// (set) Token: 0x060009D9 RID: 2521 RVA: 0x00017525 File Offset: 0x00015725
		[JsonProperty("count")]
		public string Count { get; set; }

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060009DA RID: 2522 RVA: 0x0001752E File Offset: 0x0001572E
		// (set) Token: 0x060009DB RID: 2523 RVA: 0x00017536 File Offset: 0x00015736
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060009DC RID: 2524 RVA: 0x0001753F File Offset: 0x0001573F
		// (set) Token: 0x060009DD RID: 2525 RVA: 0x00017547 File Offset: 0x00015747
		[JsonProperty("type")]
		public string Type { get; set; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060009DE RID: 2526 RVA: 0x00017550 File Offset: 0x00015750
		// (set) Token: 0x060009DF RID: 2527 RVA: 0x00017558 File Offset: 0x00015758
		[JsonProperty("role_name")]
		public string RoleName { get; set; }
	}
}
