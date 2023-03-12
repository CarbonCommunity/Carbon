/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000F0 RID: 240
	public class Properties
	{
		// Token: 0x040004FB RID: 1275
		[JsonProperty("$os")]
		public string OS;

		// Token: 0x040004FC RID: 1276
		[JsonProperty("$browser")]
		public string Browser;

		// Token: 0x040004FD RID: 1277
		[JsonProperty("$device")]
		public string Device;
	}
}
