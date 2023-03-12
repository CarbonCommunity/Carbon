/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000CA RID: 202
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GatewayHelloEvent
	{
		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x000151CA File Offset: 0x000133CA
		// (set) Token: 0x0600076D RID: 1901 RVA: 0x000151D2 File Offset: 0x000133D2
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval { get; set; }
	}
}
