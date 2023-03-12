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
	// Token: 0x020000AC RID: 172
	public enum GuildNsfwLevel
	{
		// Token: 0x040003EB RID: 1003
		[System.ComponentModel.Description("DEFAULT")]
		Default,
		// Token: 0x040003EC RID: 1004
		[System.ComponentModel.Description("EXPLICIT")]
		Explicit,
		// Token: 0x040003ED RID: 1005
		[System.ComponentModel.Description("SAFE")]
		Safe,
		// Token: 0x040003EE RID: 1006
		[System.ComponentModel.Description("AGE_RESTRICTED")]
		AgeRestricted
	}
}
