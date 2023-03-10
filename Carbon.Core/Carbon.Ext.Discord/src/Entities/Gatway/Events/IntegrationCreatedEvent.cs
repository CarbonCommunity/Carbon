/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Integrations;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000DA RID: 218
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class IntegrationCreatedEvent : Integration
	{
		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060007C6 RID: 1990 RVA: 0x00015448 File Offset: 0x00013648
		// (set) Token: 0x060007C7 RID: 1991 RVA: 0x00015450 File Offset: 0x00013650
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
