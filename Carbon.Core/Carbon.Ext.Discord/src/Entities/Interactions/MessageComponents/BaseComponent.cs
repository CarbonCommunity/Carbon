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
	// Token: 0x02000085 RID: 133
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class BaseComponent
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x00011C75 File Offset: 0x0000FE75
		// (set) Token: 0x060004D0 RID: 1232 RVA: 0x00011C7D File Offset: 0x0000FE7D
		[JsonProperty("type")]
		public MessageComponentType Type { get; protected set; }
	}
}
