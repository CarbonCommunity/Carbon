/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
	// Token: 0x02000089 RID: 137
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InputTextComponent : BaseInteractableComponent
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00011D07 File Offset: 0x0000FF07
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x00011D0F File Offset: 0x0000FF0F
		[JsonProperty("style")]
		public InputTextStyles Style { get; set; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x00011D18 File Offset: 0x0000FF18
		// (set) Token: 0x060004E3 RID: 1251 RVA: 0x00011D20 File Offset: 0x0000FF20
		[JsonProperty("label")]
		public string Label { get; set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x00011D29 File Offset: 0x0000FF29
		// (set) Token: 0x060004E5 RID: 1253 RVA: 0x00011D31 File Offset: 0x0000FF31
		[JsonProperty("min_length")]
		public int? MinLength { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x00011D3A File Offset: 0x0000FF3A
		// (set) Token: 0x060004E7 RID: 1255 RVA: 0x00011D42 File Offset: 0x0000FF42
		[JsonProperty("max_length")]
		public int? MaxLength { get; set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x00011D4B File Offset: 0x0000FF4B
		// (set) Token: 0x060004E9 RID: 1257 RVA: 0x00011D53 File Offset: 0x0000FF53
		[JsonProperty("placeholder")]
		public string Placeholder { get; set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x00011D5C File Offset: 0x0000FF5C
		// (set) Token: 0x060004EB RID: 1259 RVA: 0x00011D64 File Offset: 0x0000FF64
		[JsonProperty("value")]
		public string Value { get; set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x00011D6D File Offset: 0x0000FF6D
		// (set) Token: 0x060004ED RID: 1261 RVA: 0x00011D75 File Offset: 0x0000FF75
		[JsonProperty("required")]
		public bool? Required { get; set; }

		// Token: 0x060004EE RID: 1262 RVA: 0x00011D7E File Offset: 0x0000FF7E
		public InputTextComponent()
		{
			base.Type = MessageComponentType.InputText;
		}
	}
}
