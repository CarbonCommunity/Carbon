/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000B8 RID: 184
	public class WelcomeScreenUpdate
	{
		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x00014866 File Offset: 0x00012A66
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x0001486E File Offset: 0x00012A6E
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00014877 File Offset: 0x00012A77
		// (set) Token: 0x060006E9 RID: 1769 RVA: 0x0001487F File Offset: 0x00012A7F
		[JsonProperty("welcome_channels")]
		public List<GuildWelcomeScreenChannel> WelcomeChannels { get; set; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x00014888 File Offset: 0x00012A88
		// (set) Token: 0x060006EB RID: 1771 RVA: 0x00014890 File Offset: 0x00012A90
		[JsonProperty("description")]
		public string Description { get; set; }
	}
}
