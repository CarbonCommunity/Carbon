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
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x0200007C RID: 124
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InteractionData
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x0001159A File Offset: 0x0000F79A
		// (set) Token: 0x0600048B RID: 1163 RVA: 0x000115A2 File Offset: 0x0000F7A2
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x000115AB File Offset: 0x0000F7AB
		// (set) Token: 0x0600048D RID: 1165 RVA: 0x000115B3 File Offset: 0x0000F7B3
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x000115BC File Offset: 0x0000F7BC
		// (set) Token: 0x0600048F RID: 1167 RVA: 0x000115C4 File Offset: 0x0000F7C4
		[JsonProperty("type")]
		public ApplicationCommandType? Type { get; set; }

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000490 RID: 1168 RVA: 0x000115CD File Offset: 0x0000F7CD
		// (set) Token: 0x06000491 RID: 1169 RVA: 0x000115D5 File Offset: 0x0000F7D5
		[JsonProperty("resolved")]
		public InteractionDataResolved Resolved { get; set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000492 RID: 1170 RVA: 0x000115DE File Offset: 0x0000F7DE
		// (set) Token: 0x06000493 RID: 1171 RVA: 0x000115E6 File Offset: 0x0000F7E6
		[JsonProperty("options")]
		public List<InteractionDataOption> Options { get; set; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x000115EF File Offset: 0x0000F7EF
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x000115F7 File Offset: 0x0000F7F7
		[JsonProperty("custom_id")]
		public string CustomId { get; set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00011600 File Offset: 0x0000F800
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x00011608 File Offset: 0x0000F808
		[JsonProperty("component_type")]
		public MessageComponentType? ComponentType { get; set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x00011611 File Offset: 0x0000F811
		// (set) Token: 0x06000499 RID: 1177 RVA: 0x00011619 File Offset: 0x0000F819
		[JsonProperty("values")]
		public List<string> Values { get; set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x00011622 File Offset: 0x0000F822
		// (set) Token: 0x0600049B RID: 1179 RVA: 0x0001162A File Offset: 0x0000F82A
		[JsonProperty("target_id")]
		public Snowflake? TargetId { get; set; }
	}
}
