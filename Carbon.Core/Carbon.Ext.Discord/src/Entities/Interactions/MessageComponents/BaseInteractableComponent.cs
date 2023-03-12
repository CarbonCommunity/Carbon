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
	// Token: 0x02000086 RID: 134
	public class BaseInteractableComponent : BaseComponent
	{
		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060004D2 RID: 1234 RVA: 0x00011C86 File Offset: 0x0000FE86
		// (set) Token: 0x060004D3 RID: 1235 RVA: 0x00011C8E File Offset: 0x0000FE8E
		[JsonProperty("custom_id")]
		public string CustomId { get; set; }
	}
}
