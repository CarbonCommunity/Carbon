/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000F3 RID: 243
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class UpdateVoiceStatusCommand
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600087C RID: 2172 RVA: 0x000159E0 File Offset: 0x00013BE0
		// (set) Token: 0x0600087D RID: 2173 RVA: 0x000159E8 File Offset: 0x00013BE8
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x000159F1 File Offset: 0x00013BF1
		// (set) Token: 0x0600087F RID: 2175 RVA: 0x000159F9 File Offset: 0x00013BF9
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x00015A02 File Offset: 0x00013C02
		// (set) Token: 0x06000881 RID: 2177 RVA: 0x00015A0A File Offset: 0x00013C0A
		[JsonProperty("self_mute")]
		public bool SelfMute { get; set; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000882 RID: 2178 RVA: 0x00015A13 File Offset: 0x00013C13
		// (set) Token: 0x06000883 RID: 2179 RVA: 0x00015A1B File Offset: 0x00013C1B
		[JsonProperty("self_deaf")]
		public bool SelfDeaf { get; set; }
	}
}
