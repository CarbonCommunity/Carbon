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

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000F7 RID: 247
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ChannelCreate
	{
		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x00015C96 File Offset: 0x00013E96
		// (set) Token: 0x060008A0 RID: 2208 RVA: 0x00015C9E File Offset: 0x00013E9E
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x00015CA7 File Offset: 0x00013EA7
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x00015CAF File Offset: 0x00013EAF
		[JsonProperty("type")]
		public ChannelType Type { get; set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00015CB8 File Offset: 0x00013EB8
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x00015CC0 File Offset: 0x00013EC0
		[JsonProperty("topic")]
		public string Topic { get; set; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x00015CC9 File Offset: 0x00013EC9
		// (set) Token: 0x060008A6 RID: 2214 RVA: 0x00015CD1 File Offset: 0x00013ED1
		[JsonProperty("bitrate")]
		public int? Bitrate { get; set; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00015CDA File Offset: 0x00013EDA
		// (set) Token: 0x060008A8 RID: 2216 RVA: 0x00015CE2 File Offset: 0x00013EE2
		[JsonProperty("user_limit")]
		public int? UserLimit { get; set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00015CEB File Offset: 0x00013EEB
		// (set) Token: 0x060008AA RID: 2218 RVA: 0x00015CF3 File Offset: 0x00013EF3
		[JsonProperty("rate_limit_per_user")]
		public int? RateLimitPerUser { get; set; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x00015CFC File Offset: 0x00013EFC
		// (set) Token: 0x060008AC RID: 2220 RVA: 0x00015D04 File Offset: 0x00013F04
		[JsonProperty("position")]
		public int? Position { get; set; }

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x00015D0D File Offset: 0x00013F0D
		// (set) Token: 0x060008AE RID: 2222 RVA: 0x00015D15 File Offset: 0x00013F15
		[JsonProperty("permission_overwrites")]
		public List<Overwrite> PermissionOverwrites { get; set; }

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x00015D1E File Offset: 0x00013F1E
		// (set) Token: 0x060008B0 RID: 2224 RVA: 0x00015D26 File Offset: 0x00013F26
		[JsonProperty("parent_id")]
		public Snowflake? ParentId { get; set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x00015D2F File Offset: 0x00013F2F
		// (set) Token: 0x060008B2 RID: 2226 RVA: 0x00015D37 File Offset: 0x00013F37
		[JsonProperty("nsfw")]
		public bool? Nsfw { get; set; }
	}
}
