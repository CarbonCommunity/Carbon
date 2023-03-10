/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Api
{
	// Token: 0x0200011A RID: 282
	public class RateLimit
	{
		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00017CF4 File Offset: 0x00015EF4
		// (set) Token: 0x06000A7E RID: 2686 RVA: 0x00017CFC File Offset: 0x00015EFC
		[JsonProperty("message")]
		public string Message { get; set; }

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000A7F RID: 2687 RVA: 0x00017D05 File Offset: 0x00015F05
		// (set) Token: 0x06000A80 RID: 2688 RVA: 0x00017D0D File Offset: 0x00015F0D
		[JsonProperty("retry_after")]
		public float RetryAfter { get; set; }

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000A81 RID: 2689 RVA: 0x00017D16 File Offset: 0x00015F16
		// (set) Token: 0x06000A82 RID: 2690 RVA: 0x00017D1E File Offset: 0x00015F1E
		[JsonProperty("global")]
		public bool Global { get; set; }
	}
}
