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
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000D3 RID: 211
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMembersChunkEvent
	{
		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x000152E3 File Offset: 0x000134E3
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x000152EB File Offset: 0x000134EB
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x000152F4 File Offset: 0x000134F4
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x000152FC File Offset: 0x000134FC
		[JsonProperty("members")]
		public List<GuildMember> Members { get; set; }

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000799 RID: 1945 RVA: 0x00015305 File Offset: 0x00013505
		// (set) Token: 0x0600079A RID: 1946 RVA: 0x0001530D File Offset: 0x0001350D
		[JsonProperty("chunk_index")]
		public int ChunkIndex { get; set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x00015316 File Offset: 0x00013516
		// (set) Token: 0x0600079C RID: 1948 RVA: 0x0001531E File Offset: 0x0001351E
		[JsonProperty("chunk_count")]
		public int ChunkCount { get; set; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x00015327 File Offset: 0x00013527
		// (set) Token: 0x0600079E RID: 1950 RVA: 0x0001532F File Offset: 0x0001352F
		[JsonProperty("not_found")]
		public List<string> NotFound { get; set; }

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x0600079F RID: 1951 RVA: 0x00015338 File Offset: 0x00013538
		// (set) Token: 0x060007A0 RID: 1952 RVA: 0x00015340 File Offset: 0x00013540
		[JsonProperty("presences")]
		public List<UpdatePresenceCommand> Presences { get; set; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x060007A1 RID: 1953 RVA: 0x00015349 File Offset: 0x00013549
		// (set) Token: 0x060007A2 RID: 1954 RVA: 0x00015351 File Offset: 0x00013551
		[JsonProperty("nonce")]
		public string Nonce { get; set; }
	}
}
