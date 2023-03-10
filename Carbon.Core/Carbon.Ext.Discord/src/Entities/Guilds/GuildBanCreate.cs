/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A4 RID: 164
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildBanCreate
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000647 RID: 1607 RVA: 0x0001415D File Offset: 0x0001235D
		// (set) Token: 0x06000648 RID: 1608 RVA: 0x00014165 File Offset: 0x00012365
		[JsonProperty("delete_message_days")]
		public int? DeleteMessageDays { get; set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000649 RID: 1609 RVA: 0x0001416E File Offset: 0x0001236E
		// (set) Token: 0x0600064A RID: 1610 RVA: 0x00014176 File Offset: 0x00012376
		[JsonProperty("reason")]
		public string Reason { get; set; }
	}
}
