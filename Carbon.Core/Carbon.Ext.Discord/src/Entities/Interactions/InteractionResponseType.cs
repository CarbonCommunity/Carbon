/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x02000082 RID: 130
	public enum InteractionResponseType
	{
		// Token: 0x040002E2 RID: 738
		Pong = 1,
		// Token: 0x040002E3 RID: 739
		ChannelMessageWithSource = 4,
		// Token: 0x040002E4 RID: 740
		DeferredChannelMessageWithSource,
		// Token: 0x040002E5 RID: 741
		DeferredUpdateMessage,
		// Token: 0x040002E6 RID: 742
		UpdateMessage,
		// Token: 0x040002E7 RID: 743
		ApplicationCommandAutocompleteResult,
		// Token: 0x040002E8 RID: 744
		Modal
	}
}
