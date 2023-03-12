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
	// Token: 0x02000087 RID: 135
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ButtonComponent : BaseInteractableComponent
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x00011CA0 File Offset: 0x0000FEA0
		// (set) Token: 0x060004D6 RID: 1238 RVA: 0x00011CA8 File Offset: 0x0000FEA8
		[JsonProperty("style")]
		public ButtonStyle Style { get; set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x00011CB1 File Offset: 0x0000FEB1
		// (set) Token: 0x060004D8 RID: 1240 RVA: 0x00011CB9 File Offset: 0x0000FEB9
		[JsonProperty("label")]
		public string Label { get; set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x00011CC2 File Offset: 0x0000FEC2
		// (set) Token: 0x060004DA RID: 1242 RVA: 0x00011CCA File Offset: 0x0000FECA
		[JsonProperty("emoji")]
		public DiscordEmoji Emoji { get; set; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x00011CD3 File Offset: 0x0000FED3
		// (set) Token: 0x060004DC RID: 1244 RVA: 0x00011CDB File Offset: 0x0000FEDB
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x00011CE4 File Offset: 0x0000FEE4
		// (set) Token: 0x060004DE RID: 1246 RVA: 0x00011CEC File Offset: 0x0000FEEC
		[JsonProperty("disabled")]
		public bool? Disabled { get; set; }

		// Token: 0x060004DF RID: 1247 RVA: 0x00011CF5 File Offset: 0x0000FEF5
		public ButtonComponent()
		{
			base.Type = MessageComponentType.Button;
		}
	}
}
