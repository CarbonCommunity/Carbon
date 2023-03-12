/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Activities
{
	// Token: 0x02000120 RID: 288
	[Flags]
	public enum ActivityFlags
	{
		// Token: 0x0400062F RID: 1583
		[System.ComponentModel.Description("NONE")]
		None = 0,
		// Token: 0x04000630 RID: 1584
		[System.ComponentModel.Description("INSTANCE")]
		Instance = 1,
		// Token: 0x04000631 RID: 1585
		[System.ComponentModel.Description("JOIN")]
		Join = 2,
		// Token: 0x04000632 RID: 1586
		[System.ComponentModel.Description("SPECTATE")]
		Spectate = 4,
		// Token: 0x04000633 RID: 1587
		[System.ComponentModel.Description("JOIN_REQUEST")]
		JoinRequest = 8,
		// Token: 0x04000634 RID: 1588
		[System.ComponentModel.Description("SYNC")]
		Sync = 16,
		// Token: 0x04000635 RID: 1589
		[System.ComponentModel.Description("PLAY")]
		Play = 32,
		// Token: 0x04000636 RID: 1590
		[System.ComponentModel.Description("PARTY_PRIVACY_FRIENDS")]
		PartyPrivacyFriends = 64,
		// Token: 0x04000637 RID: 1591
		[System.ComponentModel.Description("PARTY_PRIVACY_VOICE_CHANNEL")]
		PartyPrivacyVoiceChannel = 128,
		// Token: 0x04000638 RID: 1592
		[System.ComponentModel.Description("EMBEDDED")]
		Embedded = 256
	}
}
