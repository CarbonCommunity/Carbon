using System;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/resources/guild#guild-object-system-channel-flags">System Channel Flags</a>
	/// </summary>
	[Flags]
	public enum SystemChannelFlags
	{
		/// <summary>
		/// Suppress member join notifications
		/// </summary>
		[System.ComponentModel.Description("SUPPRESS_JOIN_NOTIFICATIONS")]
		SuppressJoinNotifications = 1 << 0,

		/// <summary>
		/// Suppress server boost notifications
		/// </summary>
		[System.ComponentModel.Description("SUPPRESS_PREMIUM_SUBSCRIPTIONS")]
		SuppressPremiumSubscriptions = 1 << 1,

		/// <summary>
		/// Suppress server setup tips
		/// </summary>
		[System.ComponentModel.Description("SUPPRESS_GUILD_REMINDER_NOTIFICATIONS")]
		SuppressGuildReminderNotifications = 1 << 1,

		/// <summary>
		/// Hide member join sticker reply buttons
		/// </summary>
		[System.ComponentModel.Description("SUPPRESS_JOIN_NOTIFICATION_REPLIES")]
		SuppressJoinNotificationReplies = 1 << 1
	}
}
