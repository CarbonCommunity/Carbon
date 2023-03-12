/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000B2 RID: 178
	public enum GuildVerificationLevel
	{
		// Token: 0x04000406 RID: 1030
		[System.ComponentModel.Description("NONE")]
		None,
		// Token: 0x04000407 RID: 1031
		[System.ComponentModel.Description("LOW")]
		Low,
		// Token: 0x04000408 RID: 1032
		[System.ComponentModel.Description("MEDIUM")]
		Medium,
		// Token: 0x04000409 RID: 1033
		[System.ComponentModel.Description("HIGH")]
		High,
		// Token: 0x0400040A RID: 1034
		[System.ComponentModel.Description("VERY_HIGH")]
		VeryHigh
	}
}
