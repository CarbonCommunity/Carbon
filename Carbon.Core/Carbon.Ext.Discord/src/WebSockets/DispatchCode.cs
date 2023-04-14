using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.WebSockets
{
	/// <summary>
	/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#commands-and-events-gateway-events">Gateway Event Codes</a>
	/// </summary>
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum DispatchCode : byte
	{
		/// <summary>
		/// Used when we don't have a matching Dispatch Code
		/// </summary>
		Unknown,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#ready">READY</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("READY")] Ready,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#resumed">RESUMED</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("RESUMED")] Resumed,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#channel-create">CHANNEL_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_CREATE")] ChannelCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#channel-update">CHANNEL_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_UPDATE")] ChannelUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#channel-delete">CHANNEL_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_DELETE")] ChannelDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#channel-pins-update">CHANNEL_PINS_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("CHANNEL_PINS_UPDATE")] ChannelPinsUpdate,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-create">GUILD_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_CREATE")] GuildCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-update">GUILD_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_UPDATE")] GuildUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-delete">GUILD_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_DELETE")] GuildDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-ban-add">GUILD_BAN_ADD</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_BAN_ADD")] GuildBanAdded,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-ban-remove">GUILD_BAN_REMOVE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_BAN_REMOVE")] GuildBanRemoved,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-emojis-update">GUILD_EMOJIS_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_EMOJIS_UPDATE")] GuildEmojisUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-stickers-update">GUILD_STICKERS_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_STICKERS_UPDATE")] GuildStickersUpdate,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-integrations-update">GUILD_INTEGRATIONS_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_INTEGRATIONS_UPDATE")] GuildIntegrationsUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-member-add">GUILD_MEMBER_ADD</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_MEMBER_ADD")] GuildMemberAdded,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-member-remove">GUILD_MEMBER_REMOVE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_MEMBER_REMOVE")] GuildMemberRemoved,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-member-update">GUILD_MEMBER_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_MEMBER_UPDATE")] GuildMemberUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-members-chunk">GUILD_MEMBERS_CHUNK</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_MEMBERS_CHUNK")] GuildMembersChunk,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-role-create">GUILD_ROLE_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_ROLE_CREATE")] GuildRoleCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-role-update">GUILD_ROLE_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_ROLE_UPDATE")] GuildRoleUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-role-delete">GUILD_ROLE_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_ROLE_DELETE")] GuildRoleDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-scheduled-event-create">GUILD_SCHEDULED_EVENT_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_CREATE")] GuildScheduledEventCreate,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-scheduled-event-update">GUILD_SCHEDULED_EVENT_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_UPDATE")] GuildScheduledEventUpdate,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-scheduled-event-delete">GUILD_SCHEDULED_EVENT_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_DELETE")] GuildScheduledEventDelete,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-scheduled-event-user-add">GUILD_SCHEDULED_EVENT_USER_ADD</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_USER_ADD")] GuildScheduledEventUserAdd,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#guild-scheduled-event-user-remove">GUILD_SCHEDULED_EVENT_USER_REMOVE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_SCHEDULED_EVENT_USER_REMOVE")] GuildScheduledEventUserRemove,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#integration-create">INTEGRATION_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("INTEGRATION_CREATE")] IntegrationCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#integration-update">INTEGRATION_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("INTEGRATION_UPDATE")] IntegrationUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#integration-delete">INTEGRATION_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("INTEGRATION_DELETE")] IntegrationDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-create">MESSAGE_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_CREATE")] MessageCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-update">MESSAGE_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_UPDATE")] MessageUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-delete">MESSAGE_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_DELETE")] MessageDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-delete-bulk">MESSAGE_DELETE_BULK</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_DELETE_BULK")] MessageBulkDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-reaction-add">MESSAGE_REACTION_ADD</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_REACTION_ADD")] MessageReactionAdded,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-reaction-remove">MESSAGE_REACTION_REMOVE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_REACTION_REMOVE")] MessageReactionRemoved,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-reaction-remove-all">MESSAGE_REACTION_REMOVE_ALL</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_REACTION_REMOVE_ALL")] MessageReactionAllRemoved,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#message-reaction-remove-emoji">MESSAGE_REACTION_REMOVE_EMOJI</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("MESSAGE_REACTION_REMOVE_EMOJI")] MessageReactionEmojiRemoved,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#presence-update">PRESENCE_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("PRESENCE_UPDATE")] PresenceUpdated,

		/// <summary>
		/// Represents the <a href="">PRESENCES_REPLACE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("PRESENCES_REPLACE")] PresenceReplace,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#typing-start">TYPING_START</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("TYPING_START")] TypingStarted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#user-update">USER_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("USER_UPDATE")] UserUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#voice-state-update">VOICE_STATE_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("VOICE_STATE_UPDATE")] VoiceStateUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#voice-server-update">VOICE_SERVER_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("VOICE_SERVER_UPDATE")] VoiceServerUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#webhooks-update">WEBHOOKS_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("WEBHOOKS_UPDATE")] WebhooksUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#invite-create">INVITE_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("INVITE_CREATE")] InviteCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#invite-delete">INVITE_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("INVITE_DELETE")] InviteDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#interaction-create">INTERACTION_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("INTERACTION_CREATE")] InteractionCreated,

		/// <summary>
		/// Represents the <a href="">GUILD_JOIN_REQUEST_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("GUILD_JOIN_REQUEST_DELETE")] GuildJoinRequestDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#thread-create">THREAD_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("THREAD_CREATE")] ThreadCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#thread-update">THREAD_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("THREAD_UPDATE")] ThreadUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#thread-delete">THREAD_DELETE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("THREAD_DELETE")] ThreadDeleted,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#thread-list-sync">THREAD_LIST_SYNC</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("THREAD_LIST_SYNC")] ThreadListSync,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#thread-member-update">THREAD_MEMBER_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("THREAD_MEMBER_UPDATE")] ThreadMemberUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#thread-members-update">THREAD_MEMBERS_UPDATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("THREAD_MEMBERS_UPDATE")] ThreadMembersUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#stage-instance-create">STAGE_INSTANCE_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("STAGE_INSTANCE_CREATE")] StageInstanceCreated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#stage-instance-update">STAGE_INSTANCE_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("STAGE_INSTANCE_UPDATE")] StageInstanceUpdated,

		/// <summary>
		/// Represents the <a href="https://discord.com/developers/docs/topics/gateway#stage-instance-delete">STAGE_INSTANCE_CREATE</a> gateway event
		/// </summary>
		[System.ComponentModel.Description("STAGE_INSTANCE_DELETE")] StageInstanceDeleted,
	}
}
