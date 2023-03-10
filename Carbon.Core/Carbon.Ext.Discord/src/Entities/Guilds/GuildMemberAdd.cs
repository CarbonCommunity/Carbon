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

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000AA RID: 170
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildMemberAdd
	{
		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x000144D1 File Offset: 0x000126D1
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x000144D9 File Offset: 0x000126D9
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x000144E2 File Offset: 0x000126E2
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x000144EA File Offset: 0x000126EA
		[JsonProperty("nick")]
		public string Nick { get; set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x000144F3 File Offset: 0x000126F3
		// (set) Token: 0x0600068B RID: 1675 RVA: 0x000144FB File Offset: 0x000126FB
		[JsonProperty("roles")]
		public List<Snowflake> Roles { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x00014504 File Offset: 0x00012704
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x0001450C File Offset: 0x0001270C
		[JsonProperty("mute")]
		public bool Mute { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x00014515 File Offset: 0x00012715
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x0001451D File Offset: 0x0001271D
		[JsonProperty("deaf")]
		public bool Deaf { get; set; }
	}
}
