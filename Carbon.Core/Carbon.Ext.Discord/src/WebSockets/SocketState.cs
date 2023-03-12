/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.WebSockets
{
	// Token: 0x0200000A RID: 10
	public enum SocketState
	{
		// Token: 0x04000080 RID: 128
		Disconnected,
		// Token: 0x04000081 RID: 129
		Connected,
		// Token: 0x04000082 RID: 130
		Connecting,
		// Token: 0x04000083 RID: 131
		PendingReconnect
	}
}
