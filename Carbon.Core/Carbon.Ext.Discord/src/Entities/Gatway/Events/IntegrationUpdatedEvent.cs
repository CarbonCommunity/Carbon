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
	// Token: 0x020000DC RID: 220
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class IntegrationUpdatedEvent : Integration
	{
		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x00015495 File Offset: 0x00013695
		// (set) Token: 0x060007D1 RID: 2001 RVA: 0x0001549D File Offset: 0x0001369D
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }
	}
}
