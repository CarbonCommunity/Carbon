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
	// Token: 0x02000114 RID: 276
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeInvite
	{
		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000A21 RID: 2593 RVA: 0x0001775F File Offset: 0x0001595F
		// (set) Token: 0x06000A22 RID: 2594 RVA: 0x00017767 File Offset: 0x00015967
		[JsonProperty("code")]
		public string Code { get; set; }

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x00017770 File Offset: 0x00015970
		// (set) Token: 0x06000A24 RID: 2596 RVA: 0x00017778 File Offset: 0x00015978
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x00017781 File Offset: 0x00015981
		// (set) Token: 0x06000A26 RID: 2598 RVA: 0x00017789 File Offset: 0x00015989
		[JsonProperty("inviter_id")]
		public Snowflake InviterId { get; set; }

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000A27 RID: 2599 RVA: 0x00017792 File Offset: 0x00015992
		// (set) Token: 0x06000A28 RID: 2600 RVA: 0x0001779A File Offset: 0x0001599A
		[JsonProperty("max_uses")]
		public int? MaxUses { get; set; }

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000A29 RID: 2601 RVA: 0x000177A3 File Offset: 0x000159A3
		// (set) Token: 0x06000A2A RID: 2602 RVA: 0x000177AB File Offset: 0x000159AB
		[JsonProperty("uses")]
		public int? Uses { get; set; }

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000A2B RID: 2603 RVA: 0x000177B4 File Offset: 0x000159B4
		// (set) Token: 0x06000A2C RID: 2604 RVA: 0x000177BC File Offset: 0x000159BC
		[JsonProperty("max_age")]
		public int? MaxAge { get; set; }

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000A2D RID: 2605 RVA: 0x000177C5 File Offset: 0x000159C5
		// (set) Token: 0x06000A2E RID: 2606 RVA: 0x000177CD File Offset: 0x000159CD
		[JsonProperty("temporary")]
		public bool Temporary { get; set; }
	}
}
