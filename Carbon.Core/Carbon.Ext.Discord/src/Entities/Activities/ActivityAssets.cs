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
	// Token: 0x0200011E RID: 286
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ActivityAssets
	{
		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000A92 RID: 2706 RVA: 0x00017DEC File Offset: 0x00015FEC
		// (set) Token: 0x06000A93 RID: 2707 RVA: 0x00017DF4 File Offset: 0x00015FF4
		[JsonProperty("large_image")]
		public string LargeImage { get; set; }

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x00017DFD File Offset: 0x00015FFD
		// (set) Token: 0x06000A95 RID: 2709 RVA: 0x00017E05 File Offset: 0x00016005
		[JsonProperty("large_text")]
		public string LargeText { get; set; }

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000A96 RID: 2710 RVA: 0x00017E0E File Offset: 0x0001600E
		// (set) Token: 0x06000A97 RID: 2711 RVA: 0x00017E16 File Offset: 0x00016016
		[JsonProperty("small_image")]
		public string SmallImage { get; set; }

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000A98 RID: 2712 RVA: 0x00017E1F File Offset: 0x0001601F
		// (set) Token: 0x06000A99 RID: 2713 RVA: 0x00017E27 File Offset: 0x00016027
		[JsonProperty("small_text")]
		public string SmallText { get; set; }
	}
}
