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

namespace Oxide.Ext.Discord.Entities.Emojis
{
	// Token: 0x020000F6 RID: 246
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmojiUpdate
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600089A RID: 2202 RVA: 0x00015C74 File Offset: 0x00013E74
		// (set) Token: 0x0600089B RID: 2203 RVA: 0x00015C7C File Offset: 0x00013E7C
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600089C RID: 2204 RVA: 0x00015C85 File Offset: 0x00013E85
		// (set) Token: 0x0600089D RID: 2205 RVA: 0x00015C8D File Offset: 0x00013E8D
		[JsonProperty("roles")]
		public List<Snowflake> Roles { get; set; }
	}
}
