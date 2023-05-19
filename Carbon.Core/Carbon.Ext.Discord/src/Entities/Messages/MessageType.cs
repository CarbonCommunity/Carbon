namespace Oxide.Ext.Discord.Entities.Messages
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/resources/channel#message-object-message-types">Message Types</a>
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// The default message type
		/// </summary>
		[System.ComponentModel.Description("DEFAULT")]
		Default = 0,

		/// <summary>
		/// The message when a recipient is added
		/// </summary>
		[System.ComponentModel.Description("RECIPIENT_ADD")]
		RecipientAdd = 1,

		/// <summary>
		/// The message when a recipient is removed
		/// </summary>
		[System.ComponentModel.Description("RECIPIENT_REMOVE")]
		RecipientRemove = 2,

		/// <summary>
		/// The message when a user is called
		/// </summary>
		[System.ComponentModel.Description("CALL")]
		Call = 3,

		/// <summary>
		/// The message when a channel name is changed
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_NAME_CHANGE")]
		ChannelNameChange = 4,

		/// <summary>
		/// The message when a channel icon is changed
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_ICON_CHANGE")]
		ChannelIconChange = 5,

		/// <summary>
		/// The message when another message is pinned
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_PINNED_MESSAGE")]
		ChannelPinnedMessage = 6,

		/// <summary>
		/// The message when a new member joined
		/// </summary>
		[System.ComponentModel.Description("GUILD_MEMBER_JOIN")]
		GuildMemberJoin = 7,

		/// <summary>
		///  The message for when a user boosts a guild
		/// </summary>
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION")]
		UserPremiumGuildSubscription = 8,

		/// <summary>
		/// The message for when a guild reaches Tier 1 of Nitro boosts
		/// </summary>
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_1")]
		UserPremiumGuildSubscriptionTier1 = 9,

		/// <summary>
		/// The message for when a guild reaches Tier 2 of Nitro boosts
		/// </summary>
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_2")]
		UserPremiumGuildSubscriptionTier2 = 10,

		/// <summary>
		/// The message for when a guild reaches Tier 3 of Nitro boosts
		/// </summary>
		[System.ComponentModel.Description("USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_3")]
		UserPremiumGuildSubscriptionTier3 = 11,

		/// <summary>
		/// The message for when a news channel subscription is added to a text channel
		/// </summary>
		[System.ComponentModel.Description("ChannelFollowAdd")]
		ChannelFollowAdd = 12,

		/// <summary>
		/// The message for when a guild discovery is disqualified
		/// </summary>
		[System.ComponentModel.Description("GuildDiscoveryDisqualified")]
		GuildDiscoveryDisqualified = 14,

		/// <summary>
		/// The message for when a guild discovery is requalified
		/// </summary>
		[System.ComponentModel.Description("GuildDiscoveryRequalified")]
		GuildDiscoveryRequalified = 15,

		/// <summary>
		/// The message for grace period initial warning
		/// </summary>
		[System.ComponentModel.Description("GUILD_DISCOVERY_GRACE_PERIOD_INITIAL_WARNING")]
		GuildDiscoveryGracePeriodInitialWarning = 16,

		/// <summary>
		/// The message for grace period final warning
		/// </summary>
		[System.ComponentModel.Description("GUILD_DISCOVERY_GRACE_PERIOD_FINAL_WARNING")]
		GuildDiscoveryGracePeriodFinalWarning = 17,

		/// <summary>
		/// The message created a thread
		/// </summary>
		[System.ComponentModel.Description("THREAD_CREATED")]
		ThreadCreated = 18,

		/// <summary>
		/// The message for when the message is a reply
		/// </summary>
		[System.ComponentModel.Description("REPLY")]
		Reply = 19,

		/// <summary>
		/// The message for when the message is an application command
		/// </summary>
		[System.ComponentModel.Description("CHAT_INPUT_COMMAND")]
		ChatInputCommand = 20,

		/// <summary>
		/// Starter message for a thread
		/// </summary>
		[System.ComponentModel.Description("THREAD_STARTER_MESSAGE")]
		ThreadStarterMessage = 21,

		/// <summary>
		/// Reminder for a guild invite
		/// </summary>
		[System.ComponentModel.Description("GUILD_INVITE_REMINDER")]
		GuildInviteReminder = 22,

		/// <summary>
		/// Reminder for a guild invite
		/// </summary>
		[System.ComponentModel.Description("CONTEXT_MENU_COMMAND")]
		ContextMenuCommand = 23
	}
}
