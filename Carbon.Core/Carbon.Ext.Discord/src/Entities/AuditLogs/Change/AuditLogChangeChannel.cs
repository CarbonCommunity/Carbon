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
using Oxide.Ext.Discord.Entities.Channels;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
	// Token: 0x02000111 RID: 273
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeChannel
	{
		// Token: 0x17000377 RID: 887
		// (get) Token: 0x060009E6 RID: 2534 RVA: 0x00017583 File Offset: 0x00015783
		// (set) Token: 0x060009E7 RID: 2535 RVA: 0x0001758B File Offset: 0x0001578B
		[JsonProperty("position")]
		public int? Position { get; set; }

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x060009E8 RID: 2536 RVA: 0x00017594 File Offset: 0x00015794
		// (set) Token: 0x060009E9 RID: 2537 RVA: 0x0001759C File Offset: 0x0001579C
		[JsonProperty("topic")]
		public string Topic { get; set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x060009EA RID: 2538 RVA: 0x000175A5 File Offset: 0x000157A5
		// (set) Token: 0x060009EB RID: 2539 RVA: 0x000175AD File Offset: 0x000157AD
		[JsonProperty("bitrate")]
		public int? Bitrate { get; set; }

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x060009EC RID: 2540 RVA: 0x000175B6 File Offset: 0x000157B6
		// (set) Token: 0x060009ED RID: 2541 RVA: 0x000175BE File Offset: 0x000157BE
		[JsonProperty("permission_overwrites")]
		public List<Overwrite> PermissionOverwrites { get; set; }

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x060009EE RID: 2542 RVA: 0x000175C7 File Offset: 0x000157C7
		// (set) Token: 0x060009EF RID: 2543 RVA: 0x000175CF File Offset: 0x000157CF
		[JsonProperty("nsfw")]
		public bool Nsfw { get; set; }

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060009F0 RID: 2544 RVA: 0x000175D8 File Offset: 0x000157D8
		// (set) Token: 0x060009F1 RID: 2545 RVA: 0x000175E0 File Offset: 0x000157E0
		[JsonProperty("application_id")]
		public Snowflake ApplicationId { get; set; }

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x060009F2 RID: 2546 RVA: 0x000175E9 File Offset: 0x000157E9
		// (set) Token: 0x060009F3 RID: 2547 RVA: 0x000175F1 File Offset: 0x000157F1
		[JsonProperty("rate_limit_per_user")]
		public int? RateLimitPerUser { get; set; }
	}
}
