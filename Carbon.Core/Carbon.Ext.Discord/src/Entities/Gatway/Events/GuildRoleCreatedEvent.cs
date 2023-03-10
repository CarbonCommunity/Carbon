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
	// Token: 0x020000D4 RID: 212
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildRoleCreatedEvent
	{
		// Token: 0x17000289 RID: 649
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x0001535A File Offset: 0x0001355A
		// (set) Token: 0x060007A5 RID: 1957 RVA: 0x00015362 File Offset: 0x00013562
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x060007A6 RID: 1958 RVA: 0x0001536B File Offset: 0x0001356B
		// (set) Token: 0x060007A7 RID: 1959 RVA: 0x00015373 File Offset: 0x00013573
		[JsonProperty("role")]
		public DiscordRole Role { get; set; }
	}
}
