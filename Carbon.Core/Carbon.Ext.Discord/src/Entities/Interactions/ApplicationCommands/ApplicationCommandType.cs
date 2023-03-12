/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x0200008E RID: 142
	public enum ApplicationCommandType
	{
		// Token: 0x04000317 RID: 791
		[System.ComponentModel.Description("CHAT_INPUT")]
		ChatInput = 1,
		// Token: 0x04000318 RID: 792
		[System.ComponentModel.Description("USER")]
		User,
		// Token: 0x04000319 RID: 793
		[System.ComponentModel.Description("MESSAGE")]
		Message
	}
}
