/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x02000102 RID: 258
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadChannelUpdate
	{
		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x00016F21 File Offset: 0x00015121
		// (set) Token: 0x06000964 RID: 2404 RVA: 0x00016F29 File Offset: 0x00015129
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000965 RID: 2405 RVA: 0x00016F32 File Offset: 0x00015132
		// (set) Token: 0x06000966 RID: 2406 RVA: 0x00016F3A File Offset: 0x0001513A
		[JsonProperty("archived")]
		public bool Archived { get; set; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06000967 RID: 2407 RVA: 0x00016F43 File Offset: 0x00015143
		// (set) Token: 0x06000968 RID: 2408 RVA: 0x00016F4B File Offset: 0x0001514B
		[JsonProperty("auto_archive_duration")]
		public int AutoArchiveDuration { get; set; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000969 RID: 2409 RVA: 0x00016F54 File Offset: 0x00015154
		// (set) Token: 0x0600096A RID: 2410 RVA: 0x00016F5C File Offset: 0x0001515C
		[JsonProperty("locked")]
		public bool Locked { get; set; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x0600096B RID: 2411 RVA: 0x00016F65 File Offset: 0x00015165
		// (set) Token: 0x0600096C RID: 2412 RVA: 0x00016F6D File Offset: 0x0001516D
		[JsonProperty("invitable")]
		public bool Invitable { get; set; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600096D RID: 2413 RVA: 0x00016F76 File Offset: 0x00015176
		// (set) Token: 0x0600096E RID: 2414 RVA: 0x00016F7E File Offset: 0x0001517E
		[JsonProperty("rate_limit_per_user")]
		public int? RateLimitPerUser { get; set; }
	}
}
