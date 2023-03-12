/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
	// Token: 0x02000113 RID: 275
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeIntegration
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000A1A RID: 2586 RVA: 0x0001772C File Offset: 0x0001592C
		// (set) Token: 0x06000A1B RID: 2587 RVA: 0x00017734 File Offset: 0x00015934
		[JsonProperty("enable_emoticons")]
		public bool EnableEmoticons { get; set; }

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000A1C RID: 2588 RVA: 0x0001773D File Offset: 0x0001593D
		// (set) Token: 0x06000A1D RID: 2589 RVA: 0x00017745 File Offset: 0x00015945
		[JsonProperty("expire_behavior")]
		public int ExpireBehavior { get; set; }

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000A1E RID: 2590 RVA: 0x0001774E File Offset: 0x0001594E
		// (set) Token: 0x06000A1F RID: 2591 RVA: 0x00017756 File Offset: 0x00015956
		[JsonProperty("expire_grace_period")]
		public int ExpireGracePeriod { get; set; }
	}
}
