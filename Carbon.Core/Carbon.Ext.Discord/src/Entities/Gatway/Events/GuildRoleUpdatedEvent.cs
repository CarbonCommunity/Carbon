/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000D6 RID: 214
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildRoleUpdatedEvent
	{
		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060007AE RID: 1966 RVA: 0x0001539E File Offset: 0x0001359E
		// (set) Token: 0x060007AF RID: 1967 RVA: 0x000153A6 File Offset: 0x000135A6
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x000153AF File Offset: 0x000135AF
		// (set) Token: 0x060007B1 RID: 1969 RVA: 0x000153B7 File Offset: 0x000135B7
		[JsonProperty("role")]
		public DiscordRole Role { get; set; }
	}
}
