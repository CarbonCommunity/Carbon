/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000FD RID: 253
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class FollowedChannel
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000935 RID: 2357 RVA: 0x00016DBC File Offset: 0x00014FBC
		// (set) Token: 0x06000936 RID: 2358 RVA: 0x00016DC4 File Offset: 0x00014FC4
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x00016DCD File Offset: 0x00014FCD
		// (set) Token: 0x06000938 RID: 2360 RVA: 0x00016DD5 File Offset: 0x00014FD5
		[JsonProperty("webhook_id")]
		public Snowflake WebhookId { get; set; }
	}
}
