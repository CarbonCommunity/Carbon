/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Users
{
	// Token: 0x0200004F RID: 79
	public enum UserStatusType
	{
		// Token: 0x04000167 RID: 359
		[System.ComponentModel.Description("online")]
		Online,
		// Token: 0x04000168 RID: 360
		[System.ComponentModel.Description("dnd")]
		DND,
		// Token: 0x04000169 RID: 361
		[System.ComponentModel.Description("idle")]
		Idle,
		// Token: 0x0400016A RID: 362
		[System.ComponentModel.Description("invisible")]
		Invisible,
		// Token: 0x0400016B RID: 363
		[System.ComponentModel.Description("offline")]
		Offline
	}
}
