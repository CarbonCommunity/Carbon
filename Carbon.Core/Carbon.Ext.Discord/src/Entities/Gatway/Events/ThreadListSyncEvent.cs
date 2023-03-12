/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E6 RID: 230
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ThreadListSyncEvent
	{
		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x00015757 File Offset: 0x00013957
		// (set) Token: 0x0600082D RID: 2093 RVA: 0x0001575F File Offset: 0x0001395F
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x00015768 File Offset: 0x00013968
		// (set) Token: 0x0600082F RID: 2095 RVA: 0x00015770 File Offset: 0x00013970
		[JsonProperty("channel_ids")]
		public List<Snowflake> ChannelIds { get; set; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000830 RID: 2096 RVA: 0x00015779 File Offset: 0x00013979
		// (set) Token: 0x06000831 RID: 2097 RVA: 0x00015781 File Offset: 0x00013981
		[JsonConverter(typeof(HashListConverter<DiscordChannel>))]
		[JsonProperty("threads")]
		public Hash<Snowflake, DiscordChannel> Threads { get; set; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x0001578A File Offset: 0x0001398A
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x00015792 File Offset: 0x00013992
		[JsonProperty("members")]
		public List<ThreadMember> Members { get; set; }
	}
}
