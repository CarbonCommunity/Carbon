/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000099 RID: 153
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildCommandPermissions
	{
		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x000122A7 File Offset: 0x000104A7
		// (set) Token: 0x06000555 RID: 1365 RVA: 0x000122AF File Offset: 0x000104AF
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x000122B8 File Offset: 0x000104B8
		// (set) Token: 0x06000557 RID: 1367 RVA: 0x000122C0 File Offset: 0x000104C0
		[JsonProperty("application_id")]
		public Snowflake ApplicationId { get; set; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x000122C9 File Offset: 0x000104C9
		// (set) Token: 0x06000559 RID: 1369 RVA: 0x000122D1 File Offset: 0x000104D1
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x000122DA File Offset: 0x000104DA
		// (set) Token: 0x0600055B RID: 1371 RVA: 0x000122E2 File Offset: 0x000104E2
		[JsonProperty("permissions")]
		public List<CommandPermissions> Permissions { get; set; }
	}
}
