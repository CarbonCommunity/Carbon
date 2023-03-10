/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000062 RID: 98
	public enum MessageActivityType
	{
		// Token: 0x04000217 RID: 535
		[System.ComponentModel.Description("JOIN")]
		Join = 1,
		// Token: 0x04000218 RID: 536
		[System.ComponentModel.Description("SPECTATE")]
		Spectate,
		// Token: 0x04000219 RID: 537
		[System.ComponentModel.Description("LISTEN")]
		Listen,
		// Token: 0x0400021A RID: 538
		[System.ComponentModel.Description("JOIN_REQUEST")]
		JoinRequest = 5
	}
}
