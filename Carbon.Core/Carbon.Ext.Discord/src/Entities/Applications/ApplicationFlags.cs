/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Applications
{
	// Token: 0x02000117 RID: 279
	[Flags]
	public enum ApplicationFlags
	{
		// Token: 0x040005F9 RID: 1529
		None = 0,
		// Token: 0x040005FA RID: 1530
		GatewayPresence = 4096,
		// Token: 0x040005FB RID: 1531
		GatewayPresenceLimited = 8192,
		// Token: 0x040005FC RID: 1532
		GatewayGuildMembers = 16384,
		// Token: 0x040005FD RID: 1533
		GatewayGuildMembersLimited = 32768,
		// Token: 0x040005FE RID: 1534
		VerificationPendingGuildLimit = 65536,
		// Token: 0x040005FF RID: 1535
		Embedded = 131072,
		// Token: 0x04000600 RID: 1536
		GatewayMessageContent = 262144,
		// Token: 0x04000601 RID: 1537
		GatewayMessageContentLimited = 524288
	}
}
