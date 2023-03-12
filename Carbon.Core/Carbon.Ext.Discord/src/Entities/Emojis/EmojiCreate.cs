/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Emojis
{
	// Token: 0x020000F5 RID: 245
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmojiCreate : EmojiUpdate
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000897 RID: 2199 RVA: 0x00015C63 File Offset: 0x00013E63
		// (set) Token: 0x06000898 RID: 2200 RVA: 0x00015C6B File Offset: 0x00013E6B
		[JsonProperty("image")]
		public string ImageData { get; set; }
	}
}
