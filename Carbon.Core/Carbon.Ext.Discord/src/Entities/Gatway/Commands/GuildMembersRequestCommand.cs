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

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000EE RID: 238
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMembersRequestCommand
	{
		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000861 RID: 2145 RVA: 0x000158E7 File Offset: 0x00013AE7
		// (set) Token: 0x06000862 RID: 2146 RVA: 0x000158EF File Offset: 0x00013AEF
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000863 RID: 2147 RVA: 0x000158F8 File Offset: 0x00013AF8
		// (set) Token: 0x06000864 RID: 2148 RVA: 0x00015900 File Offset: 0x00013B00
		[JsonProperty("query")]
		public string Query { get; set; } = "";

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000865 RID: 2149 RVA: 0x00015909 File Offset: 0x00013B09
		// (set) Token: 0x06000866 RID: 2150 RVA: 0x00015911 File Offset: 0x00013B11
		[JsonProperty("limit")]
		public int Limit { get; set; } = 0;

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000867 RID: 2151 RVA: 0x0001591A File Offset: 0x00013B1A
		// (set) Token: 0x06000868 RID: 2152 RVA: 0x00015922 File Offset: 0x00013B22
		[JsonProperty("presences")]
		public bool? Presences { get; set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000869 RID: 2153 RVA: 0x0001592B File Offset: 0x00013B2B
		// (set) Token: 0x0600086A RID: 2154 RVA: 0x00015933 File Offset: 0x00013B33
		[JsonProperty("user_ids")]
		public List<Snowflake> UserIds { get; set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x0600086B RID: 2155 RVA: 0x0001593C File Offset: 0x00013B3C
		// (set) Token: 0x0600086C RID: 2156 RVA: 0x00015944 File Offset: 0x00013B44
		[JsonProperty("nonce")]
		public string Nonce { get; set; }
	}
}
