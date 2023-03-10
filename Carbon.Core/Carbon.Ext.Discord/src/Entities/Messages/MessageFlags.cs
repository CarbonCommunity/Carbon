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
	// Token: 0x02000066 RID: 102
	[Flags]
	public enum MessageFlags
	{
		// Token: 0x04000234 RID: 564
		None = 0,
		// Token: 0x04000235 RID: 565
		[System.ComponentModel.Description("CROSSPOSTED")]
		CrossPosted = 1,
		// Token: 0x04000236 RID: 566
		[System.ComponentModel.Description("IS_CROSSPOST")]
		IsCrossPost = 2,
		// Token: 0x04000237 RID: 567
		[System.ComponentModel.Description("SUPPRESS_EMBEDS")]
		SuppressEmbeds = 4,
		// Token: 0x04000238 RID: 568
		[System.ComponentModel.Description("SOURCE_MESSAGE_DELETED")]
		SourceMessageDeleted = 8,
		// Token: 0x04000239 RID: 569
		[System.ComponentModel.Description("URGENT")]
		Urgent = 16,
		// Token: 0x0400023A RID: 570
		[System.ComponentModel.Description("HAS_THREAD")]
		HasThread = 32,
		// Token: 0x0400023B RID: 571
		[System.ComponentModel.Description("EPHEMERAL")]
		Ephemeral = 64,
		// Token: 0x0400023C RID: 572
		[System.ComponentModel.Description("LOADING")]
		Loading = 128
	}
}
