/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Activities
{
	// Token: 0x0200011F RID: 287
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ActivityButton
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00017E30 File Offset: 0x00016030
		// (set) Token: 0x06000A9C RID: 2716 RVA: 0x00017E38 File Offset: 0x00016038
		[JsonProperty("label")]
		public string Label { get; set; }

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000A9D RID: 2717 RVA: 0x00017E41 File Offset: 0x00016041
		// (set) Token: 0x06000A9E RID: 2718 RVA: 0x00017E49 File Offset: 0x00016049
		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
