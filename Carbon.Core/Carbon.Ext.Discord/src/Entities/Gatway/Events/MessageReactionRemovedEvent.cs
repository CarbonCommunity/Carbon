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

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E4 RID: 228
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageReactionRemovedEvent
	{
		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x000156AD File Offset: 0x000138AD
		// (set) Token: 0x06000817 RID: 2071 RVA: 0x000156B5 File Offset: 0x000138B5
		[JsonProperty("user_id")]
		public Snowflake UserId { get; set; }

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x000156BE File Offset: 0x000138BE
		// (set) Token: 0x06000819 RID: 2073 RVA: 0x000156C6 File Offset: 0x000138C6
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x000156CF File Offset: 0x000138CF
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x000156D7 File Offset: 0x000138D7
		[JsonProperty("message_id")]
		public Snowflake MessageId { get; set; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x000156E0 File Offset: 0x000138E0
		// (set) Token: 0x0600081D RID: 2077 RVA: 0x000156E8 File Offset: 0x000138E8
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x000156F1 File Offset: 0x000138F1
		// (set) Token: 0x0600081F RID: 2079 RVA: 0x000156F9 File Offset: 0x000138F9
		[JsonProperty("emoji")]
		public DiscordEmoji Emoji { get; set; }
	}
}
