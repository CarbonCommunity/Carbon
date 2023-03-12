/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000B7 RID: 183
	[Flags]
	public enum SystemChannelFlags
	{
		// Token: 0x0400041A RID: 1050
		[System.ComponentModel.Description("SUPPRESS_JOIN_NOTIFICATIONS")]
		SuppressJoinNotifications = 1,
		// Token: 0x0400041B RID: 1051
		[System.ComponentModel.Description("SUPPRESS_PREMIUM_SUBSCRIPTIONS")]
		SuppressPremiumSubscriptions = 2,
		// Token: 0x0400041C RID: 1052
		[System.ComponentModel.Description("SUPPRESS_GUILD_REMINDER_NOTIFICATIONS")]
		SuppressGuildReminderNotifications = 2,
		// Token: 0x0400041D RID: 1053
		[System.ComponentModel.Description("SUPPRESS_JOIN_NOTIFICATION_REPLIES")]
		SuppressJoinNotificationReplies = 2
	}
}
