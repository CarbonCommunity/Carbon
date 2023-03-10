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
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000F2 RID: 242
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class UpdatePresenceCommand
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x00015979 File Offset: 0x00013B79
		// (set) Token: 0x06000874 RID: 2164 RVA: 0x00015981 File Offset: 0x00013B81
		[JsonProperty("status")]
		public UserStatusType Status { get; set; } = UserStatusType.Online;

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x0001598A File Offset: 0x00013B8A
		// (set) Token: 0x06000876 RID: 2166 RVA: 0x00015992 File Offset: 0x00013B92
		[JsonProperty("activities")]
		public List<DiscordActivity> Activities { get; set; }

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000877 RID: 2167 RVA: 0x0001599B File Offset: 0x00013B9B
		// (set) Token: 0x06000878 RID: 2168 RVA: 0x000159A3 File Offset: 0x00013BA3
		[JsonProperty("since")]
		public int? Since { get; set; } = new int?(0);

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000879 RID: 2169 RVA: 0x000159AC File Offset: 0x00013BAC
		// (set) Token: 0x0600087A RID: 2170 RVA: 0x000159B4 File Offset: 0x00013BB4
		[JsonProperty("afk")]
		public bool Afk { get; set; } = false;
	}
}
