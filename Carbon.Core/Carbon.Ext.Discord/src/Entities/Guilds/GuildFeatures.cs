using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/resources/guild#guild-object-guild-features">Guild Features</a>
	/// </summary>
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum GuildFeatures
	{

		/// <summary>
		/// Discord Extension doesn't currently support a guild features
		/// </summary>
		Unknown,

		/// <summary>
		/// Guild has access to set an animated guild icon
		/// </summary>
		[System.ComponentModel.Description("ANIMATED_ICON")]
		AnimatedIcon,

		/// <summary>
		/// Guild has access to set a guild banner image
		/// </summary>
		[System.ComponentModel.Description("BANNER")]
		Banner,

		/// <summary>
		/// Guild has access to use commerce features (i.e. create store channels)
		/// </summary>
		[System.ComponentModel.Description("COMMERCE")]
		Commerce,

		/// <summary>
		/// Guild can enable welcome screen and discovery, and receives community updates
		/// </summary>
		[System.ComponentModel.Description("COMMUNITY")]
		Community,

		/// <summary>
		/// Guild is lurkable and able to be discovered in the directory
		/// </summary>
		[System.ComponentModel.Description("DISCOVERABLE")]
		Discoverable,

		/// <summary>
		/// Guild is able to be featured in the directory
		/// </summary>
		[System.ComponentModel.Description("FEATURABLE")]
		Featurable,

		/// <summary>
		/// Guild has access to set an invite splash background
		/// </summary>
		[System.ComponentModel.Description("INVITE_SPLASH")]
		InviteSplash,

		/// <summary>
		/// Guild has enabled Membership Screening
		/// </summary>
		[System.ComponentModel.Description("MEMBER_VERIFICATION_GATE_ENABLED")]
		MemberVerificationGateEnabled,

		/// <summary>
		/// Guild has enabled monetization
		/// </summary>
		[System.ComponentModel.Description("MONETIZATION_ENABLED")]
		MonetizationEnabled,

		/// <summary>
		/// Guild has increased custom sticker slots
		/// </summary>
		[System.ComponentModel.Description("MORE_STICKERS")]
		MoreStickers,

		/// <summary>
		/// Guild has access to create news channels
		/// </summary>
		[System.ComponentModel.Description("NEWS")]
		News,

		/// <summary>
		/// Guild is partnered
		/// </summary>
		[System.ComponentModel.Description("PARTNERED")]
		Partnered,

		/// <summary>
		/// Guild can be previewed before joining via Membership Screening or the directory
		/// </summary>
		[System.ComponentModel.Description("PREVIEW_ENABLED")]
		PreviewEnabled,

		/// <summary>
		/// Guild has access to create private threads
		/// </summary>
		[System.ComponentModel.Description("PRIVATE_THREADS")]
		PrivateThreads,

		/// <summary>
		/// Guild can be previewed before joining via Membership Screening or the directory
		/// </summary>
		[System.ComponentModel.Description("ROLE_ICONS")]
		RoleIcons,

		/// <summary>
		/// Guild has access to the seven day archive time for threads
		/// </summary>
		[System.ComponentModel.Description("SEVEN_DAY_THREAD_ARCHIVE")]
		SevenDayThreadArchive,

		/// <summary>
		/// Guild has access to the three day archive time for threads
		/// </summary>
		[System.ComponentModel.Description("THREE_DAY_THREAD_ARCHIVE")]
		ThreeDayThreadArchive,

		/// <summary>
		/// Guild has enabled ticketed events
		/// </summary>
		[System.ComponentModel.Description("TICKETED_EVENTS_ENABLED")]
		TicketedEventsEnabled,

		/// <summary>
		/// Guild has access to set a vanity URL
		/// </summary>
		[System.ComponentModel.Description("VANITY_URL")]
		VanityUrl,

		/// <summary>
		/// Guild is verified
		/// </summary>
		[System.ComponentModel.Description("VERIFIED")]
		Verified,

		/// <summary>
		/// Guild has access to set 384kbps bitrate in voice (previously VIP voice servers)
		/// </summary>
		[System.ComponentModel.Description("VIP_REGIONS")]
		VipRegions,

		/// <summary>
		/// Guild has enabled the welcome screen
		/// </summary>
		[System.ComponentModel.Description("WELCOME_SCREEN_ENABLED")]
		WelcomeScreenEnabled,
	}
}
