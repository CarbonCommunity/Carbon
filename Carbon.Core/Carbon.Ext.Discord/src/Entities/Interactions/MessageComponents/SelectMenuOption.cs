/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
	// Token: 0x0200008D RID: 141
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class SelectMenuOption
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x00011DF9 File Offset: 0x0000FFF9
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x00011E01 File Offset: 0x00010001
		[JsonProperty("label")]
		public string Label { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x00011E0A File Offset: 0x0001000A
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x00011E12 File Offset: 0x00010012
		[JsonProperty("value")]
		public string Value { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x00011E1B File Offset: 0x0001001B
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x00011E23 File Offset: 0x00010023
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x00011E2C File Offset: 0x0001002C
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x00011E34 File Offset: 0x00010034
		[JsonProperty("emoji")]
		public DiscordEmoji Emoji { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000501 RID: 1281 RVA: 0x00011E3D File Offset: 0x0001003D
		// (set) Token: 0x06000502 RID: 1282 RVA: 0x00011E45 File Offset: 0x00010045
		[JsonProperty("default")]
		public bool? Default { get; set; }
	}
}
