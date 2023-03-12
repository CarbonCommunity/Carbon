/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Channels.Stages
{
	// Token: 0x02000109 RID: 265
	public enum PrivacyLevel
	{
		// Token: 0x04000587 RID: 1415
		[Obsolete("Deprecated by Discord")]
		[System.ComponentModel.Description("PUBLIC")]
		Public = 1,
		// Token: 0x04000588 RID: 1416
		[System.ComponentModel.Description("GUILD_ONLY")]
		GuildOnly
	}
}
