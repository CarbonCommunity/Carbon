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
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000B5 RID: 181
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildWidget
	{
		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x000147DE File Offset: 0x000129DE
		// (set) Token: 0x060006D5 RID: 1749 RVA: 0x000147E6 File Offset: 0x000129E6
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x000147EF File Offset: 0x000129EF
		// (set) Token: 0x060006D7 RID: 1751 RVA: 0x000147F7 File Offset: 0x000129F7
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00014800 File Offset: 0x00012A00
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x00014808 File Offset: 0x00012A08
		[JsonProperty("instant_invite")]
		public string InstantInvite { get; set; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x00014811 File Offset: 0x00012A11
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x00014819 File Offset: 0x00012A19
		[JsonProperty("channels")]
		public List<DiscordChannel> Channels { get; set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00014822 File Offset: 0x00012A22
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x0001482A File Offset: 0x00012A2A
		[JsonProperty("members")]
		public List<DiscordUser> Members { get; set; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00014833 File Offset: 0x00012A33
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x0001483B File Offset: 0x00012A3B
		[JsonProperty("presence_count")]
		public int PresenceCount { get; set; }
	}
}
