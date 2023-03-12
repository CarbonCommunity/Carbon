/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Channels
{
	// Token: 0x020000FB RID: 251
	public enum ChannelType
	{
		// Token: 0x0400052C RID: 1324
		GuildText,
		// Token: 0x0400052D RID: 1325
		Dm,
		// Token: 0x0400052E RID: 1326
		GuildVoice,
		// Token: 0x0400052F RID: 1327
		GroupDm,
		// Token: 0x04000530 RID: 1328
		GuildCategory,
		// Token: 0x04000531 RID: 1329
		GuildNews,
		// Token: 0x04000532 RID: 1330
		GuildStore,
		// Token: 0x04000533 RID: 1331
		GuildNewsThread = 10,
		// Token: 0x04000534 RID: 1332
		GuildPublicThread,
		// Token: 0x04000535 RID: 1333
		GuildPrivateThread,
		// Token: 0x04000536 RID: 1334
		GuildStageVoice
	}
}
