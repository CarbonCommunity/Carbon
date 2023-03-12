/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Invites;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000F8 RID: 248
	[JsonObject(MemberSerialization = (MemberSerialization) 1)]
	public class ChannelInvite
	{
		// Token: 0x170002FF RID: 767
		// (get) Token: 0x060008B4 RID: 2228 RVA: 0x00015D40 File Offset: 0x00013F40
		// (set) Token: 0x060008B5 RID: 2229 RVA: 0x00015D48 File Offset: 0x00013F48
		[JsonProperty("max_age")]
		public int MaxAge { get; set; } = 86400;

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00015D51 File Offset: 0x00013F51
		// (set) Token: 0x060008B7 RID: 2231 RVA: 0x00015D59 File Offset: 0x00013F59
		[JsonProperty("max_uses")]
		public int MaxUses { get; set; }

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x00015D62 File Offset: 0x00013F62
		// (set) Token: 0x060008B9 RID: 2233 RVA: 0x00015D6A File Offset: 0x00013F6A
		[JsonProperty("temporary")]
		public bool Temporary { get; set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x060008BA RID: 2234 RVA: 0x00015D73 File Offset: 0x00013F73
		// (set) Token: 0x060008BB RID: 2235 RVA: 0x00015D7B File Offset: 0x00013F7B
		[JsonProperty("unique")]
		public bool Unique { get; set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060008BC RID: 2236 RVA: 0x00015D84 File Offset: 0x00013F84
		// (set) Token: 0x060008BD RID: 2237 RVA: 0x00015D8C File Offset: 0x00013F8C
		[JsonProperty("target_user_type")]
		public TargetUserType? TargetUserType { get; set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x00015D95 File Offset: 0x00013F95
		// (set) Token: 0x060008BF RID: 2239 RVA: 0x00015D9D File Offset: 0x00013F9D
		[JsonProperty("target_user_id")]
		public Snowflake TargetUser { get; set; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060008C0 RID: 2240 RVA: 0x00015DA6 File Offset: 0x00013FA6
		// (set) Token: 0x060008C1 RID: 2241 RVA: 0x00015DAE File Offset: 0x00013FAE
		[JsonProperty("target_application_id")]
		public Snowflake TargetApplicationId { get; set; }
	}
}
