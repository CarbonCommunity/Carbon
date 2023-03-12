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
using Oxide.Ext.Discord.Entities.Activities;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000E5 RID: 229
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class PresenceUpdatedEvent
	{
		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000821 RID: 2081 RVA: 0x00015702 File Offset: 0x00013902
		// (set) Token: 0x06000822 RID: 2082 RVA: 0x0001570A File Offset: 0x0001390A
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x00015713 File Offset: 0x00013913
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x0001571B File Offset: 0x0001391B
		[JsonProperty("guild_id")]
		public Snowflake GuildId { get; set; }

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000825 RID: 2085 RVA: 0x00015724 File Offset: 0x00013924
		// (set) Token: 0x06000826 RID: 2086 RVA: 0x0001572C File Offset: 0x0001392C
		[JsonProperty("status")]
		public UserStatusType Status { get; set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000827 RID: 2087 RVA: 0x00015735 File Offset: 0x00013935
		// (set) Token: 0x06000828 RID: 2088 RVA: 0x0001573D File Offset: 0x0001393D
		[JsonProperty("activities")]
		public List<DiscordActivity> Activities { get; set; }

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000829 RID: 2089 RVA: 0x00015746 File Offset: 0x00013946
		// (set) Token: 0x0600082A RID: 2090 RVA: 0x0001574E File Offset: 0x0001394E
		[JsonProperty("client_status")]
		public ClientStatus ClientStatus { get; set; }
	}
}
