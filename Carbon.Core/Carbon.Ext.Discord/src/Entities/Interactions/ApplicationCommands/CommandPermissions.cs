/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000096 RID: 150
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class CommandPermissions
	{
		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x00011F92 File Offset: 0x00010192
		// (set) Token: 0x0600052E RID: 1326 RVA: 0x00011F9A File Offset: 0x0001019A
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x00011FA3 File Offset: 0x000101A3
		// (set) Token: 0x06000530 RID: 1328 RVA: 0x00011FAB File Offset: 0x000101AB
		[JsonProperty("type")]
		public CommandPermissionType Type { get; set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000531 RID: 1329 RVA: 0x00011FB4 File Offset: 0x000101B4
		// (set) Token: 0x06000532 RID: 1330 RVA: 0x00011FBC File Offset: 0x000101BC
		[JsonProperty("permission")]
		public bool Permission { get; set; }
	}
}
