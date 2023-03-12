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

namespace Oxide.Ext.Discord.Entities.Messages.AllowedMentions
{
	// Token: 0x02000073 RID: 115
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AllowedMention
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x00010EF3 File Offset: 0x0000F0F3
		// (set) Token: 0x06000417 RID: 1047 RVA: 0x00010EFB File Offset: 0x0000F0FB
		[JsonProperty("parse")]
		public List<AllowedMentionTypes> AllowedTypes { get; set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x00010F04 File Offset: 0x0000F104
		// (set) Token: 0x06000419 RID: 1049 RVA: 0x00010F0C File Offset: 0x0000F10C
		[JsonProperty("roles")]
		public List<Snowflake> Roles { get; set; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x00010F15 File Offset: 0x0000F115
		// (set) Token: 0x0600041B RID: 1051 RVA: 0x00010F1D File Offset: 0x0000F11D
		[JsonProperty("users")]
		public List<Snowflake> Users { get; set; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x00010F26 File Offset: 0x0000F126
		// (set) Token: 0x0600041D RID: 1053 RVA: 0x00010F2E File Offset: 0x0000F12E
		[JsonProperty("replied_user")]
		public bool RepliedUser { get; set; }
	}
}
