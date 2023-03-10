/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000061 RID: 97
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageActivity
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000367 RID: 871 RVA: 0x00010711 File Offset: 0x0000E911
		// (set) Token: 0x06000368 RID: 872 RVA: 0x00010719 File Offset: 0x0000E919
		[JsonProperty("type")]
		public MessageActivityType Type { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000369 RID: 873 RVA: 0x00010722 File Offset: 0x0000E922
		// (set) Token: 0x0600036A RID: 874 RVA: 0x0001072A File Offset: 0x0000E92A
		[JsonProperty("party_id")]
		public string PartyId { get; set; }
	}
}
