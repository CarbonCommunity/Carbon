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
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E1 RID: 225
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageReactionAddedEvent
	{
		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x000155FA File Offset: 0x000137FA
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x00015602 File Offset: 0x00013802
		[JsonProperty("user_id")]
		public Snowflake UserId { get; set; }

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x0001560B File Offset: 0x0001380B
		// (set) Token: 0x06000802 RID: 2050 RVA: 0x00015613 File Offset: 0x00013813
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x0001561C File Offset: 0x0001381C
		// (set) Token: 0x06000804 RID: 2052 RVA: 0x00015624 File Offset: 0x00013824
		[JsonProperty("message_id")]
		public Snowflake MessageId { get; set; }

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x0001562D File Offset: 0x0001382D
		// (set) Token: 0x06000806 RID: 2054 RVA: 0x00015635 File Offset: 0x00013835
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06000807 RID: 2055 RVA: 0x0001563E File Offset: 0x0001383E
		// (set) Token: 0x06000808 RID: 2056 RVA: 0x00015646 File Offset: 0x00013846
		[JsonProperty("member")]
		public GuildMember Member { get; set; }

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000809 RID: 2057 RVA: 0x0001564F File Offset: 0x0001384F
		// (set) Token: 0x0600080A RID: 2058 RVA: 0x00015657 File Offset: 0x00013857
		[JsonProperty("emoji")]
		public DiscordEmoji Emoji { get; set; }
	}
}
