/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x02000081 RID: 129
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InteractionResponse
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x00011C2E File Offset: 0x0000FE2E
		// (set) Token: 0x060004C9 RID: 1225 RVA: 0x00011C36 File Offset: 0x0000FE36
		[JsonProperty("type")]
		public InteractionResponseType Type { get; set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060004CA RID: 1226 RVA: 0x00011C3F File Offset: 0x0000FE3F
		// (set) Token: 0x060004CB RID: 1227 RVA: 0x00011C47 File Offset: 0x0000FE47
		[JsonProperty("data")]
		public InteractionCallbackData Data { get; set; }
	}
}
