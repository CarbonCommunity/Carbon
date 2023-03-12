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
	// Token: 0x020000AD RID: 173
	public enum GuildPremiumTier
	{
		// Token: 0x040003F0 RID: 1008
		[System.ComponentModel.Description("NONE")]
		None,
		// Token: 0x040003F1 RID: 1009
		[System.ComponentModel.Description("TIER_1")]
		Tier1,
		// Token: 0x040003F2 RID: 1010
		[System.ComponentModel.Description("TIER_2")]
		Tier2,
		// Token: 0x040003F3 RID: 1011
		[System.ComponentModel.Description("TIER_3")]
		Tier3
	}
}
