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

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000068 RID: 104
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageReaction
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060003AD RID: 941 RVA: 0x00010AFD File Offset: 0x0000ECFD
		// (set) Token: 0x060003AE RID: 942 RVA: 0x00010B05 File Offset: 0x0000ED05
		[JsonProperty("count")]
		public int Count { get; set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00010B0E File Offset: 0x0000ED0E
		// (set) Token: 0x060003B0 RID: 944 RVA: 0x00010B16 File Offset: 0x0000ED16
		[JsonProperty("me")]
		public bool Me { get; set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x00010B1F File Offset: 0x0000ED1F
		// (set) Token: 0x060003B2 RID: 946 RVA: 0x00010B27 File Offset: 0x0000ED27
		[JsonProperty("emoji")]
		public DiscordEmoji Emoji { get; set; }
	}
}
