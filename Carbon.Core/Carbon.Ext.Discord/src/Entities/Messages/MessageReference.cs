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
	// Token: 0x02000069 RID: 105
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageReference
	{
		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00010B30 File Offset: 0x0000ED30
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x00010B38 File Offset: 0x0000ED38
		[JsonProperty("message_id")]
		public Snowflake MessageId { get; set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00010B41 File Offset: 0x0000ED41
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x00010B49 File Offset: 0x0000ED49
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00010B52 File Offset: 0x0000ED52
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x00010B5A File Offset: 0x0000ED5A
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00010B63 File Offset: 0x0000ED63
		// (set) Token: 0x060003BB RID: 955 RVA: 0x00010B6B File Offset: 0x0000ED6B
		[JsonProperty("fail_if_not_exists")]
		public bool? FailIfNotExists { get; set; }
	}
}
