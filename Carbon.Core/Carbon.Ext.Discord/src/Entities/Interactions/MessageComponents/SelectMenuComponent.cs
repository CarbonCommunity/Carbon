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

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
	// Token: 0x0200008C RID: 140
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class SelectMenuComponent : BaseInteractableComponent
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060004EF RID: 1263 RVA: 0x00011D90 File Offset: 0x0000FF90
		[JsonProperty("options")]
		public List<SelectMenuOption> Options { get; } = new List<SelectMenuOption>();

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x00011D98 File Offset: 0x0000FF98
		// (set) Token: 0x060004F1 RID: 1265 RVA: 0x00011DA0 File Offset: 0x0000FFA0
		[JsonProperty("placeholder")]
		public string Placeholder { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00011DA9 File Offset: 0x0000FFA9
		// (set) Token: 0x060004F3 RID: 1267 RVA: 0x00011DB1 File Offset: 0x0000FFB1
		[JsonProperty("min_values")]
		public int? MinValues { get; set; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00011DBA File Offset: 0x0000FFBA
		// (set) Token: 0x060004F5 RID: 1269 RVA: 0x00011DC2 File Offset: 0x0000FFC2
		[JsonProperty("max_values")]
		public int? MaxValues { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x00011DCB File Offset: 0x0000FFCB
		// (set) Token: 0x060004F7 RID: 1271 RVA: 0x00011DD3 File Offset: 0x0000FFD3
		[JsonProperty("disabled")]
		public bool? Disabled { get; set; }

		// Token: 0x060004F8 RID: 1272 RVA: 0x00011DDC File Offset: 0x0000FFDC
		public SelectMenuComponent()
		{
			base.Type = MessageComponentType.SelectMenu;
		}
	}
}
