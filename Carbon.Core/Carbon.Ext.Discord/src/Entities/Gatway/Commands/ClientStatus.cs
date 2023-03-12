/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000EC RID: 236
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ClientStatus
	{
		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600085A RID: 2138 RVA: 0x000158B4 File Offset: 0x00013AB4
		// (set) Token: 0x0600085B RID: 2139 RVA: 0x000158BC File Offset: 0x00013ABC
		[JsonProperty("desktop")]
		public string Desktop { get; set; }

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x000158C5 File Offset: 0x00013AC5
		// (set) Token: 0x0600085D RID: 2141 RVA: 0x000158CD File Offset: 0x00013ACD
		[JsonProperty("mobile")]
		public string Mobile { get; set; }

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x0600085E RID: 2142 RVA: 0x000158D6 File Offset: 0x00013AD6
		// (set) Token: 0x0600085F RID: 2143 RVA: 0x000158DE File Offset: 0x00013ADE
		[JsonProperty("web")]
		public string Web { get; set; }
	}
}
