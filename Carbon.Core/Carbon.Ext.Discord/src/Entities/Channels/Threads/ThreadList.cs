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

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
	// Token: 0x02000106 RID: 262
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadList
	{
		// Token: 0x1700034C RID: 844
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x00017097 File Offset: 0x00015297
		// (set) Token: 0x06000980 RID: 2432 RVA: 0x0001709F File Offset: 0x0001529F
		[JsonProperty("threads")]
		public List<DiscordChannel> Threads { get; set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x000170A8 File Offset: 0x000152A8
		// (set) Token: 0x06000982 RID: 2434 RVA: 0x000170B0 File Offset: 0x000152B0
		[JsonProperty("members")]
		public List<ThreadMember> Members { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000983 RID: 2435 RVA: 0x000170B9 File Offset: 0x000152B9
		// (set) Token: 0x06000984 RID: 2436 RVA: 0x000170C1 File Offset: 0x000152C1
		[JsonProperty("has_more")]
		public bool HasMore { get; set; }
	}
}
