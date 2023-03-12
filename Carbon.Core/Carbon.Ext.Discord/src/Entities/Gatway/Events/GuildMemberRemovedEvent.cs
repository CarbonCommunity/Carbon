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

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000D1 RID: 209
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMemberRemovedEvent
	{
		// Token: 0x1700027F RID: 639
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x000152B0 File Offset: 0x000134B0
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x000152B8 File Offset: 0x000134B8
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x000152C1 File Offset: 0x000134C1
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x000152C9 File Offset: 0x000134C9
		[JsonProperty("user")]
		public DiscordUser User { get; set; }
	}
}
