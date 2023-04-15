using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Oxide.Ext.Discord.Entities.Activities
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-flags">Activity Flags</a>
	/// </summary>
	[Flags]
	public enum ActivityFlags
	{
		/// <summary>
		/// No Actions can be done to this activity
		/// </summary>
		[System.ComponentModel.Description("NONE")]
		None = 0,

		/// <summary>
		/// No Actions can be done to this activity
		/// </summary>
		[System.ComponentModel.Description("INSTANCE")]
		Instance = 1 << 0,

		/// <summary>
		/// Activity can be joined
		/// </summary>
		[System.ComponentModel.Description("JOIN")]
		Join = 1 << 1,

		/// <summary>
		/// Activity can be spectated
		/// </summary>
		[System.ComponentModel.Description("SPECTATE")]
		Spectate = 1 << 2,

		/// <summary>
		/// User may request to join activity
		/// </summary>
		[System.ComponentModel.Description("JOIN_REQUEST")]
		JoinRequest = 1 << 3,

		/// <summary>
		/// User can listen along in spotify
		/// </summary>
		[System.ComponentModel.Description("SYNC")]
		Sync = 1 << 4,

		/// <summary>
		/// User can play this song
		/// </summary>
		[System.ComponentModel.Description("PLAY")]
		Play = 1 << 5,

		/// <summary>
		/// User is playing an activity in a voice channel with friends
		/// </summary>
		[System.ComponentModel.Description("PARTY_PRIVACY_FRIENDS")]
		PartyPrivacyFriends = 1 << 6,

		/// <summary>
		/// User is playing an activity in a voice channel
		/// </summary>
		[System.ComponentModel.Description("PARTY_PRIVACY_VOICE_CHANNEL")]
		PartyPrivacyVoiceChannel = 1 << 7,

		/// <summary>
		/// User is playing embedded activity
		/// </summary>
		[System.ComponentModel.Description("EMBEDDED")]
		Embedded = 1 << 8,
	}
}
