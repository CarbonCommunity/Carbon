/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
	// Token: 0x02000107 RID: 263
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadMember
	{
		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000986 RID: 2438 RVA: 0x000170CA File Offset: 0x000152CA
		// (set) Token: 0x06000987 RID: 2439 RVA: 0x000170D2 File Offset: 0x000152D2
		[JsonProperty("id")]
		public Snowflake? Id { get; set; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000988 RID: 2440 RVA: 0x000170DB File Offset: 0x000152DB
		// (set) Token: 0x06000989 RID: 2441 RVA: 0x000170E3 File Offset: 0x000152E3
		[JsonProperty("user_id")]
		public Snowflake? UserId { get; set; }

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x0600098A RID: 2442 RVA: 0x000170EC File Offset: 0x000152EC
		// (set) Token: 0x0600098B RID: 2443 RVA: 0x000170F4 File Offset: 0x000152F4
		[JsonProperty("join_timestamp")]
		public DateTime JoinTimestamp { get; set; }

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x0600098C RID: 2444 RVA: 0x000170FD File Offset: 0x000152FD
		// (set) Token: 0x0600098D RID: 2445 RVA: 0x00017105 File Offset: 0x00015305
		[JsonProperty("flags")]
		public int Flags { get; set; }

		// Token: 0x0600098E RID: 2446 RVA: 0x0001710E File Offset: 0x0001530E
		internal void Update(ThreadMember update)
		{
			this.Flags = update.Flags;
		}
	}
}
