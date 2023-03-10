/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
	// Token: 0x020000ED RID: 237
	public enum GatewayCommandCode
	{
		// Token: 0x040004E8 RID: 1256
		Heartbeat = 1,
		// Token: 0x040004E9 RID: 1257
		Identify,
		// Token: 0x040004EA RID: 1258
		PresenceUpdate,
		// Token: 0x040004EB RID: 1259
		VoiceStateUpdate,
		// Token: 0x040004EC RID: 1260
		Resume = 6,
		// Token: 0x040004ED RID: 1261
		RequestGuildMembers = 8
	}
}
