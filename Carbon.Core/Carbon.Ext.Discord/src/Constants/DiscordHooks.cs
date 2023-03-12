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
	// Token: 0x02000126 RID: 294
	[Obsolete("Please switch DiscordHooks -> DiscordExtHooks due to conflicts with a plugin name. This will be removed in a future update.")]
	public static class DiscordHooks
	{
		// Token: 0x04000656 RID: 1622
		public const string OnDiscordClientConnected = "OnDiscordClientConnected";

		// Token: 0x04000657 RID: 1623
		public const string OnDiscordClientDisconnected = "OnDiscordClientDisconnected";

		// Token: 0x04000658 RID: 1624
		public const string OnDiscordClientCreated = "OnDiscordClientCreated";

		// Token: 0x04000659 RID: 1625
		public const string OnDiscordWebsocketOpened = "OnDiscordWebsocketOpened";

		// Token: 0x0400065A RID: 1626
		public const string OnDiscordWebsocketClosed = "OnDiscordWebsocketClosed";

		// Token: 0x0400065B RID: 1627
		public const string OnDiscordWebsocketErrored = "OnDiscordWebsocketErrored";

		// Token: 0x0400065C RID: 1628
		public const string OnDiscordSetupHeartbeat = "OnDiscordSetupHeartbeat";

		// Token: 0x0400065D RID: 1629
		public const string OnDiscordHeartbeatSent = "OnDiscordHeartbeatSent";

		// Token: 0x0400065E RID: 1630
		public const string OnDiscordPlayerLinked = "OnDiscordPlayerLinked";

		// Token: 0x0400065F RID: 1631
		public const string OnDiscordPlayerUnlinked = "OnDiscordPlayerUnlinked";

		// Token: 0x04000660 RID: 1632
		public const string OnDiscordGatewayReady = "OnDiscordGatewayReady";

		// Token: 0x04000661 RID: 1633
		public const string OnDiscordGatewayResumed = "OnDiscordGatewayResumed";

		// Token: 0x04000662 RID: 1634
		public const string OnDiscordDirectChannelCreated = "OnDiscordDirectChannelCreated";

		// Token: 0x04000663 RID: 1635
		public const string OnDiscordGuildChannelCreated = "OnDiscordGuildChannelCreated";

		// Token: 0x04000664 RID: 1636
		public const string OnDiscordDirectChannelUpdated = "OnDiscordDirectChannelUpdated";

		// Token: 0x04000665 RID: 1637
		public const string OnDiscordGuildChannelUpdated = "OnDiscordGuildChannelUpdated";

		// Token: 0x04000666 RID: 1638
		public const string OnDiscordDirectChannelDeleted = "OnDiscordDirectChannelDeleted";

		// Token: 0x04000667 RID: 1639
		public const string OnDiscordGuildChannelDeleted = "OnDiscordGuildChannelDeleted";

		// Token: 0x04000668 RID: 1640
		public const string OnDiscordDirectChannelPinsUpdated = "OnDiscordDirectChannelPinsUpdated";

		// Token: 0x04000669 RID: 1641
		public const string OnDiscordGuildChannelPinsUpdated = "OnDiscordGuildChannelPinsUpdated";

		// Token: 0x0400066A RID: 1642
		public const string OnDiscordGuildCreated = "OnDiscordGuildCreated";

		// Token: 0x0400066B RID: 1643
		public const string OnDiscordGuildUpdated = "OnDiscordGuildUpdated";

		// Token: 0x0400066C RID: 1644
		public const string OnDiscordGuildUnavailable = "OnDiscordGuildUnavailable";

		// Token: 0x0400066D RID: 1645
		public const string OnDiscordGuildDeleted = "OnDiscordGuildDeleted";

		// Token: 0x0400066E RID: 1646
		public const string OnDiscordGuildMemberBanned = "OnDiscordGuildMemberBanned";

		// Token: 0x0400066F RID: 1647
		public const string OnDiscordGuildMemberUnbanned = "OnDiscordGuildMemberUnbanned";

		// Token: 0x04000670 RID: 1648
		public const string OnDiscordGuildEmojisUpdated = "OnDiscordGuildEmojisUpdated";

		// Token: 0x04000671 RID: 1649
		public const string OnDiscordGuildStickersUpdated = "OnDiscordGuildStickersUpdated";

		// Token: 0x04000672 RID: 1650
		public const string OnDiscordGuildIntegrationsUpdated = "OnDiscordGuildIntegrationsUpdated";

		// Token: 0x04000673 RID: 1651
		public const string OnDiscordGuildMemberAdded = "OnDiscordGuildMemberAdded";

		// Token: 0x04000674 RID: 1652
		public const string OnDiscordGuildMemberRemoved = "OnDiscordGuildMemberRemoved";

		// Token: 0x04000675 RID: 1653
		public const string OnDiscordGuildMemberUpdated = "OnDiscordGuildMemberUpdated";

		// Token: 0x04000676 RID: 1654
		public const string OnDiscordGuildMembersLoaded = "OnDiscordGuildMembersLoaded";

		// Token: 0x04000677 RID: 1655
		public const string OnDiscordGuildMembersChunk = "OnDiscordGuildMembersChunk";

		// Token: 0x04000678 RID: 1656
		public const string OnDiscordGuildRoleCreated = "OnDiscordGuildRoleCreated";

		// Token: 0x04000679 RID: 1657
		public const string OnDiscordGuildRoleUpdated = "OnDiscordGuildRoleUpdated";

		// Token: 0x0400067A RID: 1658
		public const string OnDiscordGuildRoleDeleted = "OnDiscordGuildRoleDeleted";

		// Token: 0x0400067B RID: 1659
		public const string OnDiscordCommand = "OnDiscordCommand";

		// Token: 0x0400067C RID: 1660
		public const string OnDiscordDirectMessageCreated = "OnDiscordDirectMessageCreated";

		// Token: 0x0400067D RID: 1661
		public const string OnDiscordGuildMessageCreated = "OnDiscordGuildMessageCreated";

		// Token: 0x0400067E RID: 1662
		public const string OnDiscordDirectMessageUpdated = "OnDiscordDirectMessageUpdated";

		// Token: 0x0400067F RID: 1663
		public const string OnDiscordGuildMessageUpdated = "OnDiscordGuildMessageUpdated";

		// Token: 0x04000680 RID: 1664
		public const string OnDiscordDirectMessageDeleted = "OnDiscordDirectMessageDeleted";

		// Token: 0x04000681 RID: 1665
		public const string OnDiscordGuildMessageDeleted = "OnDiscordGuildMessageDeleted";

		// Token: 0x04000682 RID: 1666
		public const string OnDiscordDirectMessagesBulkDeleted = "OnDiscordDirectMessagesBulkDeleted";

		// Token: 0x04000683 RID: 1667
		public const string OnDiscordGuildMessagesBulkDeleted = "OnDiscordDirectMessagesBulkDeleted";

		// Token: 0x04000684 RID: 1668
		public const string OnDiscordDirectMessageReactionAdded = "OnDiscordDirectMessageReactionAdded";

		// Token: 0x04000685 RID: 1669
		public const string OnDiscordGuildMessageReactionAdded = "OnDiscordGuildMessageReactionAdded";

		// Token: 0x04000686 RID: 1670
		public const string OnDiscordDirectMessageReactionRemoved = "OnDiscordDirectMessageReactionRemoved";

		// Token: 0x04000687 RID: 1671
		public const string OnDiscordGuildMessageReactionRemoved = "OnDiscordGuildMessageReactionRemoved";

		// Token: 0x04000688 RID: 1672
		public const string OnDiscordDirectMessageReactionRemovedAll = "OnDiscordDirectMessageReactionRemoved";

		// Token: 0x04000689 RID: 1673
		public const string OnDiscordGuildMessageReactionRemovedAll = "OnDiscordGuildMessageReactionRemoved";

		// Token: 0x0400068A RID: 1674
		public const string OnDiscordDirectMessageReactionEmojiRemoved = "OnDiscordDirectMessageReactionEmojiRemoved";

		// Token: 0x0400068B RID: 1675
		public const string OnDiscordGuildMessageReactionEmojiRemoved = "OnDiscordGuildMessageReactionEmojiRemoved";

		// Token: 0x0400068C RID: 1676
		public const string OnDiscordGuildMemberPresenceUpdated = "OnDiscordGuildMemberPresenceUpdated";

		// Token: 0x0400068D RID: 1677
		public const string OnDiscordDirectTypingStarted = "OnDiscordDirectTypingStarted";

		// Token: 0x0400068E RID: 1678
		public const string OnDiscordGuildTypingStarted = "OnDiscordGuildTypingStarted";

		// Token: 0x0400068F RID: 1679
		public const string OnDiscordUserUpdated = "OnDiscordUserUpdated";

		// Token: 0x04000690 RID: 1680
		public const string OnDiscordDirectVoiceStateUpdated = "OnDiscordDirectVoiceStateUpdated";

		// Token: 0x04000691 RID: 1681
		public const string OnDiscordGuildVoiceStateUpdated = "OnDiscordGuildVoiceStateUpdated";

		// Token: 0x04000692 RID: 1682
		public const string OnDiscordGuildVoiceServerUpdated = "OnDiscordGuildVoiceServerUpdated";

		// Token: 0x04000693 RID: 1683
		public const string OnDiscordGuildWebhookUpdated = "OnDiscordGuildWebhookUpdated";

		// Token: 0x04000694 RID: 1684
		public const string OnDiscordDirectInviteCreated = "OnDiscordDirectInviteCreated";

		// Token: 0x04000695 RID: 1685
		public const string OnDiscordGuildInviteCreated = "OnDiscordGuildInviteCreated";

		// Token: 0x04000696 RID: 1686
		public const string OnDiscordDirectInviteDeleted = "OnDiscordDirectInviteDeleted";

		// Token: 0x04000697 RID: 1687
		public const string OnDiscordGuildInviteDeleted = "OnDiscordGuildInviteDeleted";

		// Token: 0x04000698 RID: 1688
		public const string OnDiscordInteractionCreated = "OnDiscordInteractionCreated";

		// Token: 0x04000699 RID: 1689
		public const string OnDiscordGuildIntegrationCreated = "OnDiscordGuildIntegrationCreated";

		// Token: 0x0400069A RID: 1690
		public const string OnDiscordGuildIntegrationUpdated = "OnDiscordGuildIntegrationUpdated";

		// Token: 0x0400069B RID: 1691
		public const string OnDiscordIntegrationDeleted = "OnDiscordIntegrationDeleted";

		// Token: 0x0400069C RID: 1692
		public const string OnDiscordGuildThreadCreated = "OnDiscordGuildThreadCreated";

		// Token: 0x0400069D RID: 1693
		public const string OnDiscordGuildThreadUpdated = "OnDiscordGuildThreadUpdated";

		// Token: 0x0400069E RID: 1694
		public const string OnDiscordGuildThreadDeleted = "OnDiscordGuildThreadDeleted";

		// Token: 0x0400069F RID: 1695
		public const string OnDiscordGuildThreadListSynced = "OnDiscordGuildThreadListSynced";

		// Token: 0x040006A0 RID: 1696
		public const string OnDiscordGuildThreadMemberUpdated = "OnDiscordGuildThreadMemberUpdated";

		// Token: 0x040006A1 RID: 1697
		public const string OnDiscordGuildThreadMembersUpdated = "OnDiscordGuildThreadMembersUpdated";

		// Token: 0x040006A2 RID: 1698
		public const string OnDiscordUnhandledCommand = "OnDiscordUnhandledCommand";

		// Token: 0x040006A3 RID: 1699
		public const string OnDiscordStageInstanceCreated = "OnDiscordStageInstanceCreated";

		// Token: 0x040006A4 RID: 1700
		public const string OnDiscordStageInstanceUpdated = "OnDiscordStageInstanceUpdated";

		// Token: 0x040006A5 RID: 1701
		public const string OnDiscordStageInstanceDeleted = "OnDiscordStageInstanceDeleted";
	}
}
