/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x02000080 RID: 128
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InteractionDataResolved
	{
		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x00011BD9 File Offset: 0x0000FDD9
		// (set) Token: 0x060004BE RID: 1214 RVA: 0x00011BE1 File Offset: 0x0000FDE1
		[JsonProperty("users")]
		public Hash<Snowflake, DiscordUser> Users { get; set; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x00011BEA File Offset: 0x0000FDEA
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x00011BF2 File Offset: 0x0000FDF2
		[JsonProperty("members")]
		public Hash<Snowflake, GuildMember> Members { get; set; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x00011BFB File Offset: 0x0000FDFB
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x00011C03 File Offset: 0x0000FE03
		[JsonProperty("roles")]
		public Hash<Snowflake, DiscordRole> Roles { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00011C0C File Offset: 0x0000FE0C
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x00011C14 File Offset: 0x0000FE14
		[JsonProperty("channels")]
		public Hash<Snowflake, DiscordChannel> Channels { get; set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060004C5 RID: 1221 RVA: 0x00011C1D File Offset: 0x0000FE1D
		// (set) Token: 0x060004C6 RID: 1222 RVA: 0x00011C25 File Offset: 0x0000FE25
		[JsonProperty("messages")]
		public Hash<Snowflake, DiscordMessage> Messages { get; set; }
	}
}
