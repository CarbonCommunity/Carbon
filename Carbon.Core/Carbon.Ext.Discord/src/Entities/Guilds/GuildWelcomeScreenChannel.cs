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
	// Token: 0x020000B4 RID: 180
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildWelcomeScreenChannel
	{
		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060006CB RID: 1739 RVA: 0x0001479A File Offset: 0x0001299A
		// (set) Token: 0x060006CC RID: 1740 RVA: 0x000147A2 File Offset: 0x000129A2
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x000147AB File Offset: 0x000129AB
		// (set) Token: 0x060006CE RID: 1742 RVA: 0x000147B3 File Offset: 0x000129B3
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x000147BC File Offset: 0x000129BC
		// (set) Token: 0x060006D0 RID: 1744 RVA: 0x000147C4 File Offset: 0x000129C4
		[JsonProperty("emoji_id")]
		public Snowflake EmojiId { get; set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x000147CD File Offset: 0x000129CD
		// (set) Token: 0x060006D2 RID: 1746 RVA: 0x000147D5 File Offset: 0x000129D5
		[JsonProperty("emoji_name")]
		public string EmojiName { get; set; }
	}
}
