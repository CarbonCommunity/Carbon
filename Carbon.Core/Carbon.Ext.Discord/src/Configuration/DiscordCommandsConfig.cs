/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Configuration
{
	// Token: 0x02000128 RID: 296
	public class DiscordCommandsConfig
	{
		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x0001807F File Offset: 0x0001627F
		// (set) Token: 0x06000AD8 RID: 2776 RVA: 0x00018087 File Offset: 0x00016287
		[JsonProperty("Command Prefixes")]
		public char[] CommandPrefixes { get; set; }
	}
}
