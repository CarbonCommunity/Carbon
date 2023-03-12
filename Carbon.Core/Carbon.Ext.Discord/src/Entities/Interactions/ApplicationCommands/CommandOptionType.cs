/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
	// Token: 0x02000094 RID: 148
	public enum CommandOptionType
	{
		// Token: 0x0400032D RID: 813
		SubCommand = 1,
		// Token: 0x0400032E RID: 814
		SubCommandGroup,
		// Token: 0x0400032F RID: 815
		String,
		// Token: 0x04000330 RID: 816
		Integer,
		// Token: 0x04000331 RID: 817
		Boolean,
		// Token: 0x04000332 RID: 818
		User,
		// Token: 0x04000333 RID: 819
		Channel,
		// Token: 0x04000334 RID: 820
		Role,
		// Token: 0x04000335 RID: 821
		Mentionable,
		// Token: 0x04000336 RID: 822
		Number
	}
}
