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
using Oxide.Ext.Discord.Entities.Channels;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000092 RID: 146
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandOption
	{
		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x00011EC6 File Offset: 0x000100C6
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x00011ECE File Offset: 0x000100CE
		[JsonProperty("type")]
		public CommandOptionType Type { get; set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x00011ED7 File Offset: 0x000100D7
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x00011EDF File Offset: 0x000100DF
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x00011EE8 File Offset: 0x000100E8
		// (set) Token: 0x06000518 RID: 1304 RVA: 0x00011EF0 File Offset: 0x000100F0
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x00011EF9 File Offset: 0x000100F9
		// (set) Token: 0x0600051A RID: 1306 RVA: 0x00011F01 File Offset: 0x00010101
		[JsonProperty("required")]
		public bool? Required { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x00011F0A File Offset: 0x0001010A
		// (set) Token: 0x0600051C RID: 1308 RVA: 0x00011F12 File Offset: 0x00010112
		[JsonProperty("autocomplete")]
		public bool? Autocomplete { get; set; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x00011F1B File Offset: 0x0001011B
		// (set) Token: 0x0600051E RID: 1310 RVA: 0x00011F23 File Offset: 0x00010123
		[JsonProperty("choices")]
		public List<CommandOptionChoice> Choices { get; set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x00011F2C File Offset: 0x0001012C
		// (set) Token: 0x06000520 RID: 1312 RVA: 0x00011F34 File Offset: 0x00010134
		[JsonProperty("options")]
		public List<CommandOption> Options { get; set; }

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x00011F3D File Offset: 0x0001013D
		// (set) Token: 0x06000522 RID: 1314 RVA: 0x00011F45 File Offset: 0x00010145
		[JsonProperty("channel_types")]
		public List<ChannelType> ChannelTypes { get; set; }

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000523 RID: 1315 RVA: 0x00011F4E File Offset: 0x0001014E
		// (set) Token: 0x06000524 RID: 1316 RVA: 0x00011F56 File Offset: 0x00010156
		[JsonProperty("min_value")]
		public double? MinValue { get; set; }

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000525 RID: 1317 RVA: 0x00011F5F File Offset: 0x0001015F
		// (set) Token: 0x06000526 RID: 1318 RVA: 0x00011F67 File Offset: 0x00010167
		[JsonProperty("max_value")]
		public double? MaxValue { get; set; }
	}
}
