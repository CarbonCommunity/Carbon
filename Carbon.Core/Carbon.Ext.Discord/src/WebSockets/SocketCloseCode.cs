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
	// Token: 0x02000008 RID: 8
	public enum SocketCloseCode
	{
		// Token: 0x04000068 RID: 104
		UnknownError = 4000,
		// Token: 0x04000069 RID: 105
		UnknownOpcode,
		// Token: 0x0400006A RID: 106
		DecodeError,
		// Token: 0x0400006B RID: 107
		NotAuthenticated,
		// Token: 0x0400006C RID: 108
		AuthenticationFailed,
		// Token: 0x0400006D RID: 109
		AlreadyAuthenticated,
		// Token: 0x0400006E RID: 110
		InvalidSequence = 4007,
		// Token: 0x0400006F RID: 111
		RateLimited,
		// Token: 0x04000070 RID: 112
		SessionTimedOut,
		// Token: 0x04000071 RID: 113
		InvalidShard,
		// Token: 0x04000072 RID: 114
		ShardingRequired,
		// Token: 0x04000073 RID: 115
		InvalidApiVersion,
		// Token: 0x04000074 RID: 116
		InvalidIntents,
		// Token: 0x04000075 RID: 117
		DisallowedIntent,
		// Token: 0x04000076 RID: 118
		UnknownCloseCode = 4999
	}
}
