/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Gatway
{
	// Token: 0x020000C6 RID: 198
	public static class GatewayConnect
	{
		// Token: 0x0400045E RID: 1118
		public const int Version = 9;

		// Token: 0x0400045F RID: 1119
		public const string Encoding = "json";

		// Token: 0x04000460 RID: 1120
		public const string Compress = "";

		// Token: 0x04000461 RID: 1121
		public static readonly string ConnectionArgs = string.Format("v={0}&encoding={1}", 9, "json");
	}
}
