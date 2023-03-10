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
	// Token: 0x020000DB RID: 219
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class IntegrationDeletedEvent
	{
		// Token: 0x17000298 RID: 664
		// (get) Token: 0x060007C9 RID: 1993 RVA: 0x00015462 File Offset: 0x00013662
		// (set) Token: 0x060007CA RID: 1994 RVA: 0x0001546A File Offset: 0x0001366A
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x060007CB RID: 1995 RVA: 0x00015473 File Offset: 0x00013673
		// (set) Token: 0x060007CC RID: 1996 RVA: 0x0001547B File Offset: 0x0001367B
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060007CD RID: 1997 RVA: 0x00015484 File Offset: 0x00013684
		// (set) Token: 0x060007CE RID: 1998 RVA: 0x0001548C File Offset: 0x0001368C
		[JsonProperty("application_id")]
		public Snowflake ApplicationId { get; set; }
	}
}
