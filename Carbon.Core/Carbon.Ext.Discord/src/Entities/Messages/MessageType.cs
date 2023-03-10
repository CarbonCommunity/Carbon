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
	// Token: 0x0200006A RID: 106
	public enum MessageType
	{
		// Token: 0x04000249 RID: 585
		[System.ComponentModel.Description("DEFAULT")]
		Default,
		// Token: 0x0400024A RID: 586
		[System.ComponentModel.Description("RECIPIENT_ADD")]
		RecipientAdd,
		// Token: 0x0400024B RID: 587
		[System.ComponentModel.Description("RECIPIENT_REMOVE")]
		RecipientRemove,
		// Token: 0x0400024C RID: 588
		[System.ComponentModel.Description("CALL")]
		Call,
		// Token: 0x0400024D RID: 589
		[System.ComponentModel.Description("CHANNEL_NAME_CHANGE")]
		ChannelNameChange,
		// Token: 0x0400024E RID: 590
		[System.ComponentModel.Description("CHANNEL_ICON_CHANGE")]
		ChannelIconChange,
		// Token: 0x0400024F RID: 591
		[System.ComponentModel.Description("CHANNEL_PINNED_MESSAGE")]
		ChannelPinnedMessage,
		// Token: 0x04000250 RID: 592
		[System.ComponentModel.Description("GUILD_MEMBER_JOIN")]
		GuildMemberJoin,
		// Token: 0x04000251 RID: 593
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION")]
		UserPremiumGuildSubscription,
		// Token: 0x04000252 RID: 594
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_1")]
		UserPremiumGuildSubscriptionTier1,
		// Token: 0x04000253 RID: 595
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_2")]
		UserPremiumGuildSubscriptionTier2,
		// Token: 0x04000254 RID: 596
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_3")]
		UserPremiumGuildSubscriptionTier3,
		// Token: 0x04000255 RID: 597
		[System.ComponentModel.Description("ChannelFollowAdd")]
		ChannelFollowAdd,
		// Token: 0x04000256 RID: 598
		[System.ComponentModel.Description("GuildDiscoveryDisqualified")]
		GuildDiscoveryDisqualified = 14,
		// Token: 0x04000257 RID: 599
		[System.ComponentModel.Description("GuildDiscoveryRequalified")]
		GuildDiscoveryRequalified,
		// Token: 0x04000258 RID: 600
		[System.ComponentModel.Description("GUILD_DISCOVERY_GRACE_PERIOD_INITIAL_WARNING")]
		GuildDiscoveryGracePeriodInitialWarning,
		// Token: 0x04000259 RID: 601
		[System.ComponentModel.Description("GUILD_DISCOVERY_GRACE_PERIOD_FINAL_WARNING")]
		GuildDiscoveryGracePeriodFinalWarning,
		// Token: 0x0400025A RID: 602
		[System.ComponentModel.Description("THREAD_CREATED")]
		ThreadCreated,
		// Token: 0x0400025B RID: 603
		[System.ComponentModel.Description("REPLY")]
		Reply,
		// Token: 0x0400025C RID: 604
		[System.ComponentModel.Description("CHAT_INPUT_COMMAND")]
		ChatInputCommand,
		// Token: 0x0400025D RID: 605
		[System.ComponentModel.Description("THREAD_STARTER_MESSAGE")]
		ThreadStarterMessage,
		// Token: 0x0400025E RID: 606
		[System.ComponentModel.Description("GUILD_INVITE_REMINDER")]
		GuildInviteReminder,
		// Token: 0x0400025F RID: 607
		[System.ComponentModel.Description("CONTEXT_MENU_COMMAND")]
		ContextMenuCommand
	}
}
