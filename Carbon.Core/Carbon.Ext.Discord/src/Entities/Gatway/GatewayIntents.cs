using System;

namespace Oxide.Ext.Discord.Entities.Gatway
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#gateway-intents">Gateway Intents</a>
    /// These are used to indicate which events your bot / application wants to listen to / have access to
    /// </summary>
    [Flags]
    public enum GatewayIntents
    {
        /// <summary>
        /// Represents No Intents
        /// </summary>
        None = 0,
        
        /// <summary>
        /// - GUILD_CREATE
        /// - GUILD_UPDATE
        /// - GUILD_DELETE
        /// - GUILD_ROLE_CREATE
        /// - GUILD_ROLE_UPDATE
        /// - GUILD_ROLE_DELETE
        /// - CHANNEL_CREATE
        /// - CHANNEL_UPDATE
        /// - CHANNEL_DELETE
        /// - CHANNEL_PINS_UPDATE
        /// - THREAD_CREATE
        /// - THREAD_UPDATE
        /// - THREAD_DELETE
        /// - THREAD_LIST_SYNC
        /// - THREAD_MEMBER_UPDATE
        /// - THREAD_MEMBERS_UPDATE
        /// - STAGE_INSTANCE_CREATE
        /// - STAGE_INSTANCE_UPDATE
        /// - STAGE_INSTANCE_DELETE
        /// </summary>
        Guilds = 1 << 0,
        
        /// <summary>
        /// - GUILD_MEMBER_ADD
        /// - GUILD_MEMBER_UPDATE
        /// - GUILD_MEMBER_REMOVE
        /// - THREAD_MEMBERS_UPDATE
        /// </summary>
        GuildMembers = 1 << 1,
        
        /// <summary>
        /// - GUILD_BAN_ADD
        /// - GUILD_BAN_REMOVE
        /// </summary>
        GuildBans = 1 << 2,
        
        /// <summary>
        /// - GUILD_EMOJIS_UPDATE
        /// - GUILD_STICKERS_UPDATE
        /// </summary>
        GuildEmojisAndStickers  = 1 << 3,
        
        /// <summary>
        /// - GUILD_INTEGRATIONS_UPDATE
        /// - INTEGRATION_CREATE
        /// - INTEGRATION_UPDATE
        /// - INTEGRATION_DELETE
        /// </summary>
        GuildIntegrations = 1 << 4,
        
        /// <summary>
        /// - WEBHOOKS_UPDATE
        /// </summary>
        GuildWebhooks = 1 << 5,
        
        /// <summary>
        /// - INVITE_CREATE
        /// - INVITE_DELETE
        /// </summary>
        GuildInvites = 1 << 6,
        
        /// <summary>
        /// - VOICE_STATE_UPDATE
        /// </summary>
        GuildVoiceStates = 1 << 7,
        
        /// <summary>
        /// - PRESENCE_UPDATE
        /// </summary>
        GuildPresences = 1 << 8,
        
        /// <summary>
        /// - MESSAGE_CREATE
        /// - MESSAGE_UPDATE
        /// - MESSAGE_DELETE
        /// - MESSAGE_DELETE_BULK
        /// </summary>
        GuildMessages = 1 << 9,
        
        /// <summary>
        /// - MESSAGE_REACTION_ADD
        /// - MESSAGE_REACTION_REMOVE
        /// - MESSAGE_REACTION_REMOVE_ALL
        /// - MESSAGE_REACTION_REMOVE_EMOJI
        /// </summary>
        GuildMessageReactions = 1 << 10,
        
        /// <summary>
        /// - TYPING_START
        /// </summary>
        GuildMessageTyping = 1 << 11,
        
        /// <summary>
        /// - MESSAGE_CREATE
        /// - MESSAGE_UPDATE
        /// - MESSAGE_DELETE
        /// - CHANNEL_PINS_UPDATE
        /// </summary>
        DirectMessages = 1 << 12,
        
        /// <summary>
        /// - MESSAGE_REACTION_ADD
        /// - MESSAGE_REACTION_REMOVE
        /// - MESSAGE_REACTION_REMOVE_ALL
        /// - MESSAGE_REACTION_REMOVE_EMOJI
        /// </summary>
        DirectMessageReactions = 1 << 13,
        
        /// <summary>
        /// - TYPING_START
        /// </summary>
        DirectMessageTyping = 1 << 14,
        
        /// <summary>
        /// - GUILD_SCHEDULED_EVENT_CREATE
        /// - GUILD_SCHEDULED_EVENT_UPDATE
        /// - GUILD_SCHEDULED_EVENT_DELETE
        /// - GUILD_SCHEDULED_EVENT_USER_ADD
        /// - GUILD_SCHEDULED_EVENT_USER_REMOVE
        /// </summary>
        GuildScheduledEvents = 1 << 16
    }
}