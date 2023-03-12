/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels.Threads;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E7 RID: 231
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadMemberUpdateEvent : ThreadMember
	{
		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x0001579B File Offset: 0x0001399B
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x000157A3 File Offset: 0x000139A3
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
