/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Gatway.Events;
using Oxide.Ext.Discord.WebSockets;

namespace Oxide.Ext.Discord.Entities.Gatway
{
	// Token: 0x020000C4 RID: 196
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EventPayload
	{
		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000751 RID: 1873 RVA: 0x00015093 File Offset: 0x00013293
		// (set) Token: 0x06000752 RID: 1874 RVA: 0x0001509B File Offset: 0x0001329B
		[JsonProperty("op")]
		public GatewayEventCode OpCode { get; internal set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000753 RID: 1875 RVA: 0x000150A4 File Offset: 0x000132A4
		// (set) Token: 0x06000754 RID: 1876 RVA: 0x000150AC File Offset: 0x000132AC
		[JsonProperty("t")]
		public JToken EventName { get; internal set; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000755 RID: 1877 RVA: 0x000150B5 File Offset: 0x000132B5
		// (set) Token: 0x06000756 RID: 1878 RVA: 0x000150BD File Offset: 0x000132BD
		[JsonProperty("d")]
		public object Data { get; internal set; }

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000757 RID: 1879 RVA: 0x000150C6 File Offset: 0x000132C6
		// (set) Token: 0x06000758 RID: 1880 RVA: 0x000150CE File Offset: 0x000132CE
		[JsonProperty("s")]
		public int? Sequence { get; internal set; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000759 RID: 1881 RVA: 0x000150D7 File Offset: 0x000132D7
		public DispatchCode EventCode
		{
			get
			{
				JToken eventName = this.EventName;
				return (eventName != null) ? eventName.ToObject<DispatchCode>() : DispatchCode.Unknown;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x000150EB File Offset: 0x000132EB
		public JObject EventData
		{
			get
			{
				return this.Data as JObject;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x0600075B RID: 1883 RVA: 0x000150F8 File Offset: 0x000132F8
		public JToken TokenData
		{
			get
			{
				return this.Data as JToken;
			}
		}
	}
}
