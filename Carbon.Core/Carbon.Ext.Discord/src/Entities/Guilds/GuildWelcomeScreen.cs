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
	// Token: 0x020000B3 RID: 179
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildWelcomeScreen
	{
		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x00014778 File Offset: 0x00012978
		// (set) Token: 0x060006C7 RID: 1735 RVA: 0x00014780 File Offset: 0x00012980
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00014789 File Offset: 0x00012989
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x00014791 File Offset: 0x00012991
		[JsonProperty("welcome_channels")]
		public List<GuildWelcomeScreenChannel> WelcomeChannels { get; set; }
	}
}
