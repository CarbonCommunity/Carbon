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
	// Token: 0x020000FE RID: 254
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GroupDmChannelUpdate
	{
		// Token: 0x1700032D RID: 813
		// (get) Token: 0x0600093A RID: 2362 RVA: 0x00016DDE File Offset: 0x00014FDE
		// (set) Token: 0x0600093B RID: 2363 RVA: 0x00016DE6 File Offset: 0x00014FE6
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x0600093C RID: 2364 RVA: 0x00016DEF File Offset: 0x00014FEF
		// (set) Token: 0x0600093D RID: 2365 RVA: 0x00016DF7 File Offset: 0x00014FF7
		[JsonProperty("icon")]
		public string Icon { get; set; }
	}
}
