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
	// Token: 0x020000C7 RID: 199
	[Flags]
	public enum GatewayIntents
	{
		// Token: 0x04000463 RID: 1123
		None = 0,
		// Token: 0x04000464 RID: 1124
		Guilds = 1,
		// Token: 0x04000465 RID: 1125
		GuildMembers = 2,
		// Token: 0x04000466 RID: 1126
		GuildBans = 4,
		// Token: 0x04000467 RID: 1127
		GuildEmojisAndStickers = 8,
		// Token: 0x04000468 RID: 1128
		GuildIntegrations = 16,
		// Token: 0x04000469 RID: 1129
		GuildWebhooks = 32,
		// Token: 0x0400046A RID: 1130
		GuildInvites = 64,
		// Token: 0x0400046B RID: 1131
		GuildVoiceStates = 128,
		// Token: 0x0400046C RID: 1132
		GuildPresences = 256,
		// Token: 0x0400046D RID: 1133
		GuildMessages = 512,
		// Token: 0x0400046E RID: 1134
		GuildMessageReactions = 1024,
		// Token: 0x0400046F RID: 1135
		GuildMessageTyping = 2048,
		// Token: 0x04000470 RID: 1136
		DirectMessages = 4096,
		// Token: 0x04000471 RID: 1137
		DirectMessageReactions = 8192,
		// Token: 0x04000472 RID: 1138
		DirectMessageTyping = 16384,
		// Token: 0x04000473 RID: 1139
		GuildScheduledEvents = 65536
	}
}
