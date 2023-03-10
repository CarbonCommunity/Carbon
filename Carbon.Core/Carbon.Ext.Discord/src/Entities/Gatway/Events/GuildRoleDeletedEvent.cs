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
	// Token: 0x020000D5 RID: 213
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildRoleDeletedEvent
	{
		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x0001537C File Offset: 0x0001357C
		// (set) Token: 0x060007AA RID: 1962 RVA: 0x00015384 File Offset: 0x00013584
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x0001538D File Offset: 0x0001358D
		// (set) Token: 0x060007AC RID: 1964 RVA: 0x00015395 File Offset: 0x00013595
		[JsonProperty("role_id")]
		public Snowflake RoleId { get; set; }
	}
}
