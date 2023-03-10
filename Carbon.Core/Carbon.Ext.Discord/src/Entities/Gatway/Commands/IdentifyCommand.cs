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

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000EF RID: 239
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class IdentifyCommand
	{
		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x00015968 File Offset: 0x00013B68
		// (set) Token: 0x0600086F RID: 2159 RVA: 0x00015970 File Offset: 0x00013B70
		[JsonProperty("intents")]
		public GatewayIntents Intents { get; set; }

		// Token: 0x040004F4 RID: 1268
		[JsonProperty("token")]
		public string Token;

		// Token: 0x040004F5 RID: 1269
		[JsonProperty("properties")]
		public Properties Properties;

		// Token: 0x040004F6 RID: 1270
		[JsonProperty("compress")]
		public bool? Compress;

		// Token: 0x040004F7 RID: 1271
		[JsonProperty("large_threshold")]
		public int? LargeThreshold;

		// Token: 0x040004F8 RID: 1272
		[JsonProperty("shard")]
		public List<int> Shard;

		// Token: 0x040004F9 RID: 1273
		[JsonProperty("presence")]
		public UpdatePresenceCommand PresenceUpdate;
	}
}
