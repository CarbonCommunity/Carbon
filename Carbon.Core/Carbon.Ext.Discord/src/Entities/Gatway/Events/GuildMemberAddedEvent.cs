/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000CF RID: 207
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMemberAddedEvent : GuildMember
	{
		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x00015274 File Offset: 0x00013474
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x0001527C File Offset: 0x0001347C
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
