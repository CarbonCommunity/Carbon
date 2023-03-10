/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Integrations
{
	// Token: 0x0200009E RID: 158
	public enum IntegrationType
	{
		// Token: 0x04000366 RID: 870
		[System.ComponentModel.Description("twitch")]
		Twitch,
		// Token: 0x04000367 RID: 871
		[System.ComponentModel.Description("youtube")]
		Youtube,
		// Token: 0x04000368 RID: 872
		[System.ComponentModel.Description("discord")]
		Discord
	}
}
