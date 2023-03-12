/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x02000100 RID: 256
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class Overwrite : ISnowflakeEntity
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x0600095A RID: 2394 RVA: 0x00016EDD File Offset: 0x000150DD
		// (set) Token: 0x0600095B RID: 2395 RVA: 0x00016EE5 File Offset: 0x000150E5
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x00016EEE File Offset: 0x000150EE
		// (set) Token: 0x0600095D RID: 2397 RVA: 0x00016EF6 File Offset: 0x000150F6
		[JsonProperty("type")]
		public PermissionType Type { get; set; }

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x00016EFF File Offset: 0x000150FF
		// (set) Token: 0x0600095F RID: 2399 RVA: 0x00016F07 File Offset: 0x00015107
		[JsonProperty("allow")]
		public PermissionFlags? Allow { get; set; }

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x00016F10 File Offset: 0x00015110
		// (set) Token: 0x06000961 RID: 2401 RVA: 0x00016F18 File Offset: 0x00015118
		[JsonProperty("deny")]
		public PermissionFlags? Deny { get; set; }
	}
}
