/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Oxide.Ext.Discord.Entities.Api
{
	// Token: 0x02000119 RID: 281
	public class DiscordApiError
	{
		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000A76 RID: 2678 RVA: 0x00017CC1 File Offset: 0x00015EC1
		// (set) Token: 0x06000A77 RID: 2679 RVA: 0x00017CC9 File Offset: 0x00015EC9
		[JsonProperty("code")]
		public int Code { get; set; }

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000A78 RID: 2680 RVA: 0x00017CD2 File Offset: 0x00015ED2
		// (set) Token: 0x06000A79 RID: 2681 RVA: 0x00017CDA File Offset: 0x00015EDA
		[JsonProperty("message")]
		public string Message { get; set; }

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x00017CE3 File Offset: 0x00015EE3
		// (set) Token: 0x06000A7B RID: 2683 RVA: 0x00017CEB File Offset: 0x00015EEB
		[JsonProperty("errors")]
		public JObject Errors { get; set; }
	}
}
