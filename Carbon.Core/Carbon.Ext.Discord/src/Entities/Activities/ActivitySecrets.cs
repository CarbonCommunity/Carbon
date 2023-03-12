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
	// Token: 0x02000122 RID: 290
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ActivitySecrets
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000AA7 RID: 2727 RVA: 0x00017E90 File Offset: 0x00016090
		// (set) Token: 0x06000AA8 RID: 2728 RVA: 0x00017E98 File Offset: 0x00016098
		[JsonProperty("join")]
		public string Join { get; set; }

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000AA9 RID: 2729 RVA: 0x00017EA1 File Offset: 0x000160A1
		// (set) Token: 0x06000AAA RID: 2730 RVA: 0x00017EA9 File Offset: 0x000160A9
		[JsonProperty("spectate")]
		public string Spectate { get; set; }

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000AAB RID: 2731 RVA: 0x00017EB2 File Offset: 0x000160B2
		// (set) Token: 0x06000AAC RID: 2732 RVA: 0x00017EBA File Offset: 0x000160BA
		[JsonProperty("match")]
		public string Match { get; set; }
	}
}
