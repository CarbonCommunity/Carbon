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
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x0200007D RID: 125
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InteractionDataOption
	{
		// Token: 0x17000151 RID: 337
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x00011633 File Offset: 0x0000F833
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x0001163B File Offset: 0x0000F83B
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x00011644 File Offset: 0x0000F844
		// (set) Token: 0x060004A0 RID: 1184 RVA: 0x0001164C File Offset: 0x0000F84C
		[JsonProperty("type")]
		public CommandOptionType Type { get; set; }

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x00011655 File Offset: 0x0000F855
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x0001165D File Offset: 0x0000F85D
		[JsonProperty("value")]
		public JToken Value { get; set; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x00011666 File Offset: 0x0000F866
		// (set) Token: 0x060004A4 RID: 1188 RVA: 0x0001166E File Offset: 0x0000F86E
		[JsonProperty("options")]
		public List<InteractionDataOption> Options { get; set; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x00011677 File Offset: 0x0000F877
		// (set) Token: 0x060004A6 RID: 1190 RVA: 0x0001167F File Offset: 0x0000F87F
		[JsonProperty("focused")]
		public bool? Focused { get; set; }
	}
}
