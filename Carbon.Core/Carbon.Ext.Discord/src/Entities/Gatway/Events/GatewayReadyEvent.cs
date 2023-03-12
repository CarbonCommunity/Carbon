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
using Oxide.Ext.Discord.Entities.Applications;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000CB RID: 203
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GatewayReadyEvent
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x000151DB File Offset: 0x000133DB
		// (set) Token: 0x06000770 RID: 1904 RVA: 0x000151E3 File Offset: 0x000133E3
		[JsonProperty("v")]
		public int Version { get; private set; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x000151EC File Offset: 0x000133EC
		// (set) Token: 0x06000772 RID: 1906 RVA: 0x000151F4 File Offset: 0x000133F4
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x000151FD File Offset: 0x000133FD
		// (set) Token: 0x06000774 RID: 1908 RVA: 0x00015205 File Offset: 0x00013405
		[JsonProperty("guilds")]
		[JsonConverter(typeof(HashListConverter<DiscordGuild>))]
		public Hash<Snowflake, DiscordGuild> Guilds { get; set; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000775 RID: 1909 RVA: 0x0001520E File Offset: 0x0001340E
		// (set) Token: 0x06000776 RID: 1910 RVA: 0x00015216 File Offset: 0x00013416
		[JsonProperty("session_id")]
		public string SessionId { get; set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000777 RID: 1911 RVA: 0x0001521F File Offset: 0x0001341F
		// (set) Token: 0x06000778 RID: 1912 RVA: 0x00015227 File Offset: 0x00013427
		[JsonProperty("shard")]
		public List<int> Shard { get; set; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000779 RID: 1913 RVA: 0x00015230 File Offset: 0x00013430
		// (set) Token: 0x0600077A RID: 1914 RVA: 0x00015238 File Offset: 0x00013438
		[JsonProperty("application")]
		public DiscordApplication Application { get; set; }
	}
}
