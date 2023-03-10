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
	// Token: 0x0200008F RID: 143
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandCreate
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x00011E4E File Offset: 0x0001004E
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x00011E56 File Offset: 0x00010056
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x00011E5F File Offset: 0x0001005F
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x00011E67 File Offset: 0x00010067
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x00011E70 File Offset: 0x00010070
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x00011E78 File Offset: 0x00010078
		[JsonProperty("options")]
		public List<CommandOption> Options { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x00011E81 File Offset: 0x00010081
		// (set) Token: 0x0600050B RID: 1291 RVA: 0x00011E89 File Offset: 0x00010089
		[JsonProperty("default_permission")]
		public bool? DefaultPermissions { get; set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x00011E92 File Offset: 0x00010092
		// (set) Token: 0x0600050D RID: 1293 RVA: 0x00011E9A File Offset: 0x0001009A
		[JsonProperty("type")]
		public ApplicationCommandType? Type { get; set; }
	}
}
