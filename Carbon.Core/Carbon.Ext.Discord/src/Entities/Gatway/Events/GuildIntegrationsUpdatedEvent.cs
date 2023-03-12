/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000CE RID: 206
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildIntegrationsUpdatedEvent
	{
		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x00015263 File Offset: 0x00013463
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x0001526B File Offset: 0x0001346B
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
