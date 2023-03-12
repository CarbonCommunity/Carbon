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
	// Token: 0x020000D2 RID: 210
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMemberUpdatedEvent : GuildMember
	{
		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000792 RID: 1938 RVA: 0x000152D2 File Offset: 0x000134D2
		// (set) Token: 0x06000793 RID: 1939 RVA: 0x000152DA File Offset: 0x000134DA
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
