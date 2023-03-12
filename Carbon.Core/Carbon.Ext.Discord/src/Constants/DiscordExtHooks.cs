/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;

namespace Oxide.Ext.Discord.Constants
{
	// Token: 0x02000127 RID: 295
	public static class DiscordExtHooks
	{
		// Token: 0x040006A6 RID: 1702
		public const string OnDiscordClientConnected = "OnDiscordClientConnected";

		// Token: 0x040006A7 RID: 1703
		public const string OnDiscordClientDisconnected = "OnDiscordClientDisconnected";

		// Token: 0x040006A8 RID: 1704
		public const string OnDiscordClientCreated = "OnDiscordClientCreated";

		// Token: 0x040006A9 RID: 1705
		public const string OnDiscordWebsocketOpened = "OnDiscordWebsocketOpened";

		// Token: 0x040006AA RID: 1706
		public const string OnDiscordWebsocketClosed = "OnDiscordWebsocketClosed";

		// Token: 0x040006AB RID: 1707
		public const string OnDiscordWebsocketErrored = "OnDiscordWebsocketErrored";

		// Token: 0x040006AC RID: 1708
		public const string OnDiscordSetupHeartbeat = "OnDiscordSetupHeartbeat";

		// Token: 0x040006AD RID: 1709
		public const string OnDiscordHeartbeatSent = "OnDiscordHeartbeatSent";

		// Token: 0x040006AE RID: 1710
		public const string OnDiscordPlayerLinked = "OnDiscordPlayerLinked";

		// Token: 0x040006AF RID: 1711
		public const string OnDiscordPlayerUnlinked = "OnDiscordPlayerUnlinked";

		// Token: 0x040006B0 RID: 1712
		public const string OnDiscordGatewayReady = "OnDiscordGatewayReady";

		// Token: 0x040006B1 RID: 1713
		public const string OnDiscordGatewayResumed = "OnDiscordGatewayResumed";

		// Token: 0x040006B2 RID: 1714
		public const string OnDiscordDirectChannelCreated = "OnDiscordDirectChannelCreated";

		// Token: 0x040006B3 RID: 1715
		public const string OnDiscordGuildChannelCreated = "OnDiscordGuildChannelCreated";

		// Token: 0x040006B4 RID: 1716
		public const string OnDiscordDirectChannelUpdated = "OnDiscordDirectChannelUpdated";

		// Token: 0x040006B5 RID: 1717
		public const string OnDiscordGuildChannelUpdated = "OnDiscordGuildChannelUpdated";

		// Token: 0x040006B6 RID: 1718
		public const string OnDiscordDirectChannelDeleted = "OnDiscordDirectChannelDeleted";

		// Token: 0x040006B7 RID: 1719
		public const string OnDiscordGuildChannelDeleted = "OnDiscordGuildChannelDeleted";

		// Token: 0x040006B8 RID: 1720
		public const string OnDiscordDirectChannelPinsUpdated = "OnDiscordDirectChannelPinsUpdated";

		// Token: 0x040006B9 RID: 1721
		public const string OnDiscordGuildChannelPinsUpdated = "OnDiscordGuildChannelPinsUpdated";

		// Token: 0x040006BA RID: 1722
		public const string OnDiscordGuildCreated = "OnDiscordGuildCreated";

		// Token: 0x040006BB RID: 1723
		public const string OnDiscordGuildUpdated = "OnDiscordGuildUpdated";

		// Token: 0x040006BC RID: 1724
		public const string OnDiscordGuildUnavailable = "OnDiscordGuildUnavailable";

		// Token: 0x040006BD RID: 1725
		public const string OnDiscordGuildDeleted = "OnDiscordGuildDeleted";

		// Token: 0x040006BE RID: 1726
		public const string OnDiscordGuildMemberBanned = "OnDiscordGuildMemberBanned";

		// Token: 0x040006BF RID: 1727
		public const string OnDiscordGuildMemberUnbanned = "OnDiscordGuildMemberUnbanned";

		// Token: 0x040006C0 RID: 1728
		public const string OnDiscordGuildEmojisUpdated = "OnDiscordGuildEmojisUpdated";

		// Token: 0x040006C1 RID: 1729
		public const string OnDiscordGuildStickersUpdated = "OnDiscordGuildStickersUpdated";

		// Token: 0x040006C2 RID: 1730
		public const string OnDiscordGuildIntegrationsUpdated = "OnDiscordGuildIntegrationsUpdated";

		// Token: 0x040006C3 RID: 1731
		public const string OnDiscordGuildMemberAdded = "OnDiscordGuildMemberAdded";

		// Token: 0x040006C4 RID: 1732
		public const string OnDiscordGuildMemberRemoved = "OnDiscordGuildMemberRemoved";

		// Token: 0x040006C5 RID: 1733
		public const string OnDiscordGuildMemberUpdated = "OnDiscordGuildMemberUpdated";

		// Token: 0x040006C6 RID: 1734
		public const string OnDiscordGuildMembersLoaded = "OnDiscordGuildMembersLoaded";

		// Token: 0x040006C7 RID: 1735
		public const string OnDiscordGuildMembersChunk = "OnDiscordGuildMembersChunk";

		// Token: 0x040006C8 RID: 1736
		public const string OnDiscordGuildRoleCreated = "OnDiscordGuildRoleCreated";

		// Token: 0x040006C9 RID: 1737
		public const string OnDiscordGuildRoleUpdated = "OnDiscordGuildRoleUpdated";

		// Token: 0x040006CA RID: 1738
		public const string OnDiscordGuildRoleDeleted = "OnDiscordGuildRoleDeleted";

		// Token: 0x040006CB RID: 1739
		public const string OnDiscordGuildScheduledEventCreated = "OnDiscordGuildScheduledEventCreated";

		// Token: 0x040006CC RID: 1740
		public const string OnDiscordGuildScheduledEventUpdated = "OnDiscordGuildScheduledEventUpdated";

		// Token: 0x040006CD RID: 1741
		public const string OnDiscordGuildScheduledEventDeleted = "OnDiscordGuildScheduledEventDeleted";

		// Token: 0x040006CE RID: 1742
		public const string OnDiscordGuildScheduledEventUserAdded = "OnDiscordGuildScheduledEventUserAdded";

		// Token: 0x040006CF RID: 1743
		public const string OnDiscordGuildScheduledEventUserRemoved = "OnDiscordGuildScheduledEventUserRemoved";

		// Token: 0x040006D0 RID: 1744
		public const string OnDiscordCommand = "OnDiscordCommand";

		// Token: 0x040006D1 RID: 1745
		public const string OnDiscordDirectMessageCreated = "OnDiscordDirectMessageCreated";

		// Token: 0x040006D2 RID: 1746
		public const string OnDiscordGuildMessageCreated = "OnDiscordGuildMessageCreated";

		// Token: 0x040006D3 RID: 1747
		public const string OnDiscordDirectMessageUpdated = "OnDiscordDirectMessageUpdated";

		// Token: 0x040006D4 RID: 1748
		public const string OnDiscordGuildMessageUpdated = "OnDiscordGuildMessageUpdated";

		// Token: 0x040006D5 RID: 1749
		public const string OnDiscordDirectMessageDeleted = "OnDiscordDirectMessageDeleted";

		// Token: 0x040006D6 RID: 1750
		public const string OnDiscordGuildMessageDeleted = "OnDiscordGuildMessageDeleted";

		// Token: 0x040006D7 RID: 1751
		public const string OnDiscordDirectMessagesBulkDeleted = "OnDiscordDirectMessagesBulkDeleted";

		// Token: 0x040006D8 RID: 1752
		public const string OnDiscordGuildMessagesBulkDeleted = "OnDiscordDirectMessagesBulkDeleted";

		// Token: 0x040006D9 RID: 1753
		public const string OnDiscordDirectMessageReactionAdded = "OnDiscordDirectMessageReactionAdded";

		// Token: 0x040006DA RID: 1754
		public const string OnDiscordGuildMessageReactionAdded = "OnDiscordGuildMessageReactionAdded";

		// Token: 0x040006DB RID: 1755
		public const string OnDiscordDirectMessageReactionRemoved = "OnDiscordDirectMessageReactionRemoved";

		// Token: 0x040006DC RID: 1756
		public const string OnDiscordGuildMessageReactionRemoved = "OnDiscordGuildMessageReactionRemoved";

		// Token: 0x040006DD RID: 1757
		public const string OnDiscordDirectMessageReactionRemovedAll = "OnDiscordDirectMessageReactionRemoved";

		// Token: 0x040006DE RID: 1758
		public const string OnDiscordGuildMessageReactionRemovedAll = "OnDiscordGuildMessageReactionRemoved";

		// Token: 0x040006DF RID: 1759
		public const string OnDiscordDirectMessageReactionEmojiRemoved = "OnDiscordDirectMessageReactionEmojiRemoved";

		// Token: 0x040006E0 RID: 1760
		public const string OnDiscordGuildMessageReactionEmojiRemoved = "OnDiscordGuildMessageReactionEmojiRemoved";

		// Token: 0x040006E1 RID: 1761
		public const string OnDiscordGuildMemberPresenceUpdated = "OnDiscordGuildMemberPresenceUpdated";

		// Token: 0x040006E2 RID: 1762
		public const string OnDiscordDirectTypingStarted = "OnDiscordDirectTypingStarted";

		// Token: 0x040006E3 RID: 1763
		public const string OnDiscordGuildTypingStarted = "OnDiscordGuildTypingStarted";

		// Token: 0x040006E4 RID: 1764
		public const string OnDiscordUserUpdated = "OnDiscordUserUpdated";

		// Token: 0x040006E5 RID: 1765
		public const string OnDiscordDirectVoiceStateUpdated = "OnDiscordDirectVoiceStateUpdated";

		// Token: 0x040006E6 RID: 1766
		public const string OnDiscordGuildVoiceStateUpdated = "OnDiscordGuildVoiceStateUpdated";

		// Token: 0x040006E7 RID: 1767
		public const string OnDiscordGuildVoiceServerUpdated = "OnDiscordGuildVoiceServerUpdated";

		// Token: 0x040006E8 RID: 1768
		public const string OnDiscordGuildWebhookUpdated = "OnDiscordGuildWebhookUpdated";

		// Token: 0x040006E9 RID: 1769
		public const string OnDiscordDirectInviteCreated = "OnDiscordDirectInviteCreated";

		// Token: 0x040006EA RID: 1770
		public const string OnDiscordGuildInviteCreated = "OnDiscordGuildInviteCreated";

		// Token: 0x040006EB RID: 1771
		public const string OnDiscordDirectInviteDeleted = "OnDiscordDirectInviteDeleted";

		// Token: 0x040006EC RID: 1772
		public const string OnDiscordGuildInviteDeleted = "OnDiscordGuildInviteDeleted";

		// Token: 0x040006ED RID: 1773
		public const string OnDiscordInteractionCreated = "OnDiscordInteractionCreated";

		// Token: 0x040006EE RID: 1774
		public const string OnDiscordGuildIntegrationCreated = "OnDiscordGuildIntegrationCreated";

		// Token: 0x040006EF RID: 1775
		public const string OnDiscordGuildIntegrationUpdated = "OnDiscordGuildIntegrationUpdated";

		// Token: 0x040006F0 RID: 1776
		public const string OnDiscordIntegrationDeleted = "OnDiscordIntegrationDeleted";

		// Token: 0x040006F1 RID: 1777
		public const string OnDiscordGuildThreadCreated = "OnDiscordGuildThreadCreated";

		// Token: 0x040006F2 RID: 1778
		public const string OnDiscordGuildThreadUpdated = "OnDiscordGuildThreadUpdated";

		// Token: 0x040006F3 RID: 1779
		public const string OnDiscordGuildThreadDeleted = "OnDiscordGuildThreadDeleted";

		// Token: 0x040006F4 RID: 1780
		public const string OnDiscordGuildThreadListSynced = "OnDiscordGuildThreadListSynced";

		// Token: 0x040006F5 RID: 1781
		public const string OnDiscordGuildThreadMemberUpdated = "OnDiscordGuildThreadMemberUpdated";

		// Token: 0x040006F6 RID: 1782
		public const string OnDiscordGuildThreadMembersUpdated = "OnDiscordGuildThreadMembersUpdated";

		// Token: 0x040006F7 RID: 1783
		public const string OnDiscordUnhandledCommand = "OnDiscordUnhandledCommand";

		// Token: 0x040006F8 RID: 1784
		public const string OnDiscordStageInstanceCreated = "OnDiscordStageInstanceCreated";

		// Token: 0x040006F9 RID: 1785
		public const string OnDiscordStageInstanceUpdated = "OnDiscordStageInstanceUpdated";

		// Token: 0x040006FA RID: 1786
		public const string OnDiscordStageInstanceDeleted = "OnDiscordStageInstanceDeleted";
	}
}
