/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000C9 RID: 201
	public enum GatewayEventCode
	{
		// Token: 0x04000478 RID: 1144
		Dispatch,
		// Token: 0x04000479 RID: 1145
		Heartbeat,
		// Token: 0x0400047A RID: 1146
		Reconnect = 7,
		// Token: 0x0400047B RID: 1147
		InvalidSession = 9,
		// Token: 0x0400047C RID: 1148
		Hello,
		// Token: 0x0400047D RID: 1149
		HeartbeatAcknowledge
	}
}
