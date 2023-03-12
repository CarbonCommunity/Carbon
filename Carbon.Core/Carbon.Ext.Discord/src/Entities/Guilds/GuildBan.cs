/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A3 RID: 163
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildBan
	{
		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x0001413B File Offset: 0x0001233B
		// (set) Token: 0x06000643 RID: 1603 RVA: 0x00014143 File Offset: 0x00012343
		[JsonProperty("reason")]
		public string Reason { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x0001414C File Offset: 0x0001234C
		// (set) Token: 0x06000645 RID: 1605 RVA: 0x00014154 File Offset: 0x00012354
		[JsonProperty("user")]
		public DiscordUser User { get; set; }
	}
}
