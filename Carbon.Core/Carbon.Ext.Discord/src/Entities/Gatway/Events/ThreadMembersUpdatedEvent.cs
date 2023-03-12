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
using Oxide.Ext.Discord.Entities.Channels.Threads;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E8 RID: 232
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadMembersUpdatedEvent
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x000157B5 File Offset: 0x000139B5
		// (set) Token: 0x06000839 RID: 2105 RVA: 0x000157BD File Offset: 0x000139BD
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x0600083A RID: 2106 RVA: 0x000157C6 File Offset: 0x000139C6
		// (set) Token: 0x0600083B RID: 2107 RVA: 0x000157CE File Offset: 0x000139CE
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x0600083C RID: 2108 RVA: 0x000157D7 File Offset: 0x000139D7
		// (set) Token: 0x0600083D RID: 2109 RVA: 0x000157DF File Offset: 0x000139DF
		[JsonProperty("member_count")]
		public int MemberCount { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x0600083E RID: 2110 RVA: 0x000157E8 File Offset: 0x000139E8
		// (set) Token: 0x0600083F RID: 2111 RVA: 0x000157F0 File Offset: 0x000139F0
		[JsonProperty("added_members")]
		public List<ThreadMember> AddedMembers { get; set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000840 RID: 2112 RVA: 0x000157F9 File Offset: 0x000139F9
		// (set) Token: 0x06000841 RID: 2113 RVA: 0x00015801 File Offset: 0x00013A01
		[JsonProperty("removed_member_ids")]
		public List<Snowflake> RemovedMemberIds { get; set; }
	}
}
