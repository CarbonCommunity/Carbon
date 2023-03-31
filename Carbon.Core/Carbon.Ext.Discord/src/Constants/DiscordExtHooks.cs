using System;
namespace Oxide.Ext.Discord.Constants
{
    /// <summary>
    /// Represents all hooks available in the discord extension
    /// </summary>
    [Obsolete("Please switch DiscordHooks -> DiscordExtHooks due to conflicts with a plugin name. This will be removed in a future update.")]
    public static class DiscordHooks
    {
        #region Bot Client Hooks
        /// <code>
        /// void OnDiscordClientConnected(Plugin owner, DiscordClient client)
        /// {
        ///     Puts("OnDiscordClientConnected Works!");
        /// }
        /// </code>
        public const string OnDiscordClientConnected = nameof(OnDiscordClientConnected);
        
        /// <code>
        /// void OnDiscordClientDisconnected(Plugin owner, DiscordClient client)
        /// {
        ///     Puts("OnDiscordClientDisconnected Works!");
        /// }
        /// </code>
        public const string OnDiscordClientDisconnected = nameof(OnDiscordClientDisconnected);
        
        /// <code>
        /// void OnDiscordClientCreated()
        /// {
        ///     Puts("OnDiscordClientCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordClientCreated = nameof(OnDiscordClientCreated);
        #endregion

        #region Socket Hooks
        /// <code>
        /// void OnDiscordWebsocketOpened()
        /// {
        ///     Puts("OnDiscordWebsocketOpened Works!");
        /// }
        /// </code>
        public const string OnDiscordWebsocketOpened = nameof(OnDiscordWebsocketOpened);
        
        /// <code>
        /// void OnDiscordWebsocketClosed(string reason, ushort code, bool wasClean)
        /// {
        ///     Puts("OnDiscordWebsocketClosed Works!");
        /// }
        /// </code>
        public const string OnDiscordWebsocketClosed = nameof(OnDiscordWebsocketClosed);
        
        /// <code>
        /// void OnDiscordWebsocketErrored(Exception ex, string message)
        /// {
        ///     Puts("OnDiscordWebsocketErrored Works!");
        /// }
        /// </code>
        public const string OnDiscordWebsocketErrored = nameof(OnDiscordWebsocketErrored);
        
        /// <code>
        /// void OnDiscordSetupHeartbeat(float heartbeat)
        /// {
        ///     Puts("OnDiscordHeartbeatSent Works!");
        /// }
        /// </code>
        public const string OnDiscordSetupHeartbeat = nameof(OnDiscordSetupHeartbeat);
        
        /// <code>
        /// void OnDiscordHeartbeatSent()
        /// {
        ///     Puts("OnDiscordHeartbeatSent Works!");
        /// }
        /// </code>
        public const string OnDiscordHeartbeatSent = nameof(OnDiscordHeartbeatSent);
        #endregion

        #region Link Hooks
        /// <code>
        /// void OnDiscordPlayerLinked(IPlayer player, DiscordUser discord)
        /// {
        ///     Puts("OnDiscordPlayerLinked Works!");
        /// }
        /// </code>
        public const string OnDiscordPlayerLinked = nameof(OnDiscordPlayerLinked);
        
        /// <code>
        /// void OnDiscordPlayerUnlinked(IPlayer player, DiscordUser discord)
        /// {
        ///     Puts("OnDiscordPlayerUnlinked Works!");
        /// }
        /// </code>
        public const string OnDiscordPlayerUnlinked = nameof(OnDiscordPlayerUnlinked);
        #endregion

        #region Discord Event Hooks
        /// <code>
        /// void OnDiscordGatewayReady(GatewayReadyEvent ready)
        /// {
        ///     Puts("OnDiscordGatewayReady Works!");
        /// }
        /// </code>
        public const string OnDiscordGatewayReady = nameof(OnDiscordGatewayReady);
        
        /// <code>
        /// void OnDiscordGatewayResumed(GatewayResumedEvent resume)
        /// {
        ///     Puts("OnDiscordGatewayResumed Works!");
        /// }
        /// </code>
        public const string OnDiscordGatewayResumed = nameof(OnDiscordGatewayResumed);
        
        /// <code>
        /// void OnDiscordDirectChannelCreated(Channel channel)
        /// {
        ///     Puts("OnDiscordDirectChannelCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelCreated = nameof(OnDiscordDirectChannelCreated);
        
        /// <code>
        /// void OnDiscordGuildChannelCreated(Channel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelCreated = nameof(OnDiscordGuildChannelCreated);
        
        /// <code>
        /// Note: previous will be null if previous channel not found
        /// void OnDiscordDirectChannelUpdated(Channel channel, DiscordChannel previous)
        /// {
        ///     Puts("OnDiscordDirectChannelUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelUpdated = nameof(OnDiscordDirectChannelUpdated);
        
        /// <code>
        /// void OnDiscordGuildChannelUpdated(DiscordChannel channel, DiscordChannel previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelUpdated = nameof(OnDiscordGuildChannelUpdated);
        
        /// <code>
        /// Note: Not sure if this will ever happen but we handle it if it does
        /// void OnDiscordDirectChannelDeleted(DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectChannelDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelDeleted = nameof(OnDiscordDirectChannelDeleted);
        
        /// <code>
        /// void OnDiscordGuildChannelDeleted(DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelDeleted = nameof(OnDiscordGuildChannelDeleted);
        
        /// <code>
        /// Note: Channel will be null if we haven't seen it yet
        /// void OnDiscordDirectChannelPinsUpdated(ChannelPinsUpdatedEvent update, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectChannelPinsUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelPinsUpdated = nameof(OnDiscordDirectChannelPinsUpdated);
        
        /// <code>
        /// void OnDiscordGuildChannelPinsUpdated(ChannelPinsUpdatedEvent update, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelPinsUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelPinsUpdated = nameof(OnDiscordGuildChannelPinsUpdated);
        
        /// <code>
        /// void OnDiscordGuildCreated(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildCreated = nameof(OnDiscordGuildCreated);
        
        /// <code>
        /// Note: previous will be null if guild previously not loaded
        /// void OnDiscordGuildUpdated(DiscordGuild guild, DiscordGuild previous)
        /// {
        ///     Puts("OnDiscordGuildUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildUpdated = nameof(OnDiscordGuildUpdated);
        
        /// <code>
        /// void OnDiscordGuildUnavailable(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildUnavailable Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildUnavailable = nameof(OnDiscordGuildUnavailable);
        
        /// <code>
        /// void OnDiscordGuildDeleted(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildDeleted = nameof(OnDiscordGuildDeleted);
        
        /// <code>
        /// void OnDiscordGuildMemberBanned(GuildMemberBannedEvent ban, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberBanned Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberBanned = nameof(OnDiscordGuildMemberBanned);
        
        /// <code>
        /// void OnDiscordGuildMemberUnbanned(GuildMemberBannedEvent ban, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildBanRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberUnbanned = nameof(OnDiscordGuildMemberUnbanned);
        
        /// <code>
        /// void OnDiscordGuildEmojisUpdated(GuildEmojisUpdatedEvent emojis, Hash&lt;Snowflake, DiscordEmoji&gt; previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildEmojisUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildEmojisUpdated = nameof(OnDiscordGuildEmojisUpdated);        
        
        /// <code>
        /// void OnDiscordGuildStickersUpdated(GuildStickersUpdatedEvent stickers, Hash&lt;Snowflake, DiscordSticker&gt; previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildStickersUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildStickersUpdated = nameof(OnDiscordGuildStickersUpdated);
        
        /// <code>
        /// void OnDiscordGuildIntegrationsUpdated(GuildIntegrationsUpdatedEvent integration, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildIntegrationsUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildIntegrationsUpdated = nameof(OnDiscordGuildIntegrationsUpdated);
        
        /// <code>
        /// void OnDiscordGuildMemberAdded(GuildMemberAddedEvent member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberAdded = nameof(OnDiscordGuildMemberAdded);
        
        /// <code>
        /// void OnDiscordGuildMemberRemoved(GuildMemberRemovedEvent member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberRemoved = nameof(OnDiscordGuildMemberRemoved);
        
        /// <code>
        /// void OnDiscordGuildMemberUpdated(GuildMemberUpdatedEvent member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberUpdated = nameof(OnDiscordGuildMemberUpdated);
        
        /// <code>
        /// void OnDiscordGuildMembersLoaded(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMembersLoaded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMembersLoaded = nameof(OnDiscordGuildMembersLoaded);
        
        /// <code>
        /// void OnDiscordGuildMembersChunk(GuildMembersChunkEvent chunk, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMembersChunk Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMembersChunk = nameof(OnDiscordGuildMembersChunk);
        
        /// <code>
        /// void OnDiscordGuildRoleCreated(DiscordRole role, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildRoleCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildRoleCreated = nameof(OnDiscordGuildRoleCreated);
        
        /// <code>
        /// void OnDiscordGuildRoleUpdated(DiscordRole role, DiscordRole previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildRoleUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildRoleUpdated = nameof(OnDiscordGuildRoleUpdated);
        
        /// <code>
        /// void OnDiscordGuildRoleDeleted(DiscordRole role, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildRoleDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildRoleDeleted = nameof(OnDiscordGuildRoleDeleted);
        
        /// <code>
        /// void OnDiscordCommand(DiscordMessage message, string cmd, string[] args)
        /// {
        ///     Puts("OnDiscordCommand Works!");
        /// }
        /// </code>
        public const string OnDiscordCommand = nameof(OnDiscordCommand);
        
        /// <code>
        /// Note: Channel may be null if we haven't seen it yet
        /// void OnDiscordDirectMessageCreated(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageCreated = nameof(OnDiscordDirectMessageCreated);
        
        /// <code>
        /// void OnDiscordGuildMessageCreated(DiscordMessage message, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageCreated = nameof(OnDiscordGuildMessageCreated);
        
        /// <code>
        /// void OnDiscordDirectMessageUpdated(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageUpdated = nameof(OnDiscordDirectMessageUpdated);
        
        /// <code>
        /// void OnDiscordDirectMessageUpdated(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageUpdated = nameof(OnDiscordGuildMessageUpdated);
        
        /// <code>
        /// void OnDiscordDirectMessageDeleted(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageDeleted = nameof(OnDiscordDirectMessageDeleted);
        
        /// <code>
        /// void OnDiscordDirectMessageDeleted(DiscordMessage message, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordDirectMessageDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageDeleted = nameof(OnDiscordGuildMessageDeleted);
        
        /// <code>
        /// void OnDiscordDirectMessagesBulkDeleted(List&lt;Snowflake&gt; messageIds, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessagesBulkDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessagesBulkDeleted = nameof(OnDiscordDirectMessagesBulkDeleted);
        
        /// <code>
        /// void OnDiscordGuildMessagesBulkDeleted(List&lt;Snowflake&gt; messageIds, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessagesBulkDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessagesBulkDeleted = nameof(OnDiscordDirectMessagesBulkDeleted);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionAdded(MessageReactionAddedEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionAdded = nameof(OnDiscordDirectMessageReactionAdded);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionAdded(MessageReactionAddedEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionAdded = nameof(OnDiscordGuildMessageReactionAdded);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionRemoved(MessageReactionRemovedEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionRemoved = nameof(OnDiscordDirectMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionRemoved(MessageReactionRemovedEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionRemoved = nameof(OnDiscordGuildMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionRemovedAll(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionRemovedAll Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionRemovedAll = nameof(OnDiscordDirectMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionRemovedAll(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionRemovedAll Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionRemovedAll = nameof(OnDiscordGuildMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionEmojiRemoved(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionEmojiRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionEmojiRemoved = nameof(OnDiscordDirectMessageReactionEmojiRemoved);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionEmojiRemoved(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionEmojiRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionEmojiRemoved = nameof(OnDiscordGuildMessageReactionEmojiRemoved);
        
        /// <code>
        /// void OnDiscordGuildMemberPresenceUpdated(PresenceUpdatedEvent update, GuildMember member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberPresenceUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberPresenceUpdated = nameof(OnDiscordGuildMemberPresenceUpdated);
        
        /// <code>
        /// void OnDiscordDirectTypingStarted(TypingStartedEvent typing, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectTypingStarted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectTypingStarted = nameof(OnDiscordDirectTypingStarted);
        
        /// <code>
        /// void OnDiscordGuildTypingStarted(TypingStartedEvent typing, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildTypingStarted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildTypingStarted = nameof(OnDiscordGuildTypingStarted);
        
        /// <code>
        /// void OnDiscordUserUpdated(DiscordUser user)
        /// {
        ///     Puts("OnDiscordUserUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordUserUpdated = nameof(OnDiscordUserUpdated);
        
        /// <code>
        /// void OnDiscordDirectVoiceStateUpdated(VoiceState voice, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectVoiceStateUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectVoiceStateUpdated = nameof(OnDiscordDirectVoiceStateUpdated);
        
        /// <code>
        /// void OnDiscordGuildVoiceStateUpdated(VoiceState voice, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildVoiceStateUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildVoiceStateUpdated = nameof(OnDiscordGuildVoiceStateUpdated);
        
        /// <code>
        /// void OnDiscordGuildVoiceServerUpdated(VoiceServerUpdatedEvent voice, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildVoiceServerUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildVoiceServerUpdated = nameof(OnDiscordGuildVoiceServerUpdated);
        
        /// <code>
        /// void OnDiscordGuildWebhookUpdated(WebhooksUpdatedEvent webhook, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildWebhookUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildWebhookUpdated = nameof(OnDiscordGuildWebhookUpdated);
        
        /// <code>
        /// void OnDiscordDirectInviteCreated(InviteCreatedEvent invite, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectInviteCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectInviteCreated = nameof(OnDiscordDirectInviteCreated);
        
        /// <code>
        /// void OnDiscordGuildInviteCreated(InviteCreatedEvent invite, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildInviteCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildInviteCreated = nameof(OnDiscordGuildInviteCreated);
        
        /// <code>
        /// void OnDiscordDirectInviteDeleted(InviteCreatedEvent invite, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectInviteDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectInviteDeleted = nameof(OnDiscordDirectInviteDeleted);
        
        /// <code>
        /// void OnDiscordGuildInviteDeleted(InviteCreatedEvent invite, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildInviteDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildInviteDeleted = nameof(OnDiscordGuildInviteDeleted);
        
        /// <code>
        /// void OnDiscordInteractionCreated(DiscordInteraction interaction)
        /// {
        ///     Puts("OnDiscordInteractionCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordInteractionCreated = nameof(OnDiscordInteractionCreated);
        
        /// <code>
        /// void OnDiscordGuildIntegrationCreated(IntegrationCreatedEvent integration, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildIntegrationCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildIntegrationCreated = nameof(OnDiscordGuildIntegrationCreated);
        
        /// <code>
        /// void OnDiscordGuildIntegrationUpdated(IntegrationUpdatedEvent interaction, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildIntegrationUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildIntegrationUpdated = nameof(OnDiscordGuildIntegrationUpdated);
        
        /// <code>
        /// void OnDiscordIntegrationDeleted(IntegrationDeletedEvent interaction, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordIntegrationDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordIntegrationDeleted = nameof(OnDiscordIntegrationDeleted);

        /// <code>
        /// void OnDiscordGuildThreadCreated(DiscordChannel thread, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadCreated = nameof(OnDiscordGuildThreadCreated);
        
        /// <code>
        /// void OnDiscordGuildThreadUpdated(DiscordChannel thread, DiscordChannel previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadUpdated = nameof(OnDiscordGuildThreadUpdated);

        /// <code>
        /// void OnDiscordGuildThreadDeleted(DiscordChannel thread, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadDeleted = nameof(OnDiscordGuildThreadDeleted);
        
        /// <code>
        /// void OnDiscordGuildThreadListSynced(ThreadListSyncEvent sync, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadListSynced Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadListSynced = nameof(OnDiscordGuildThreadListSynced);
        
        /// <code>
        /// void OnDiscordGuildThreadMemberUpdated(ThreadMember member, DiscordChannel thread, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadMemberUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadMemberUpdated = nameof(OnDiscordGuildThreadMemberUpdated);
        
        /// <code>
        /// void OnDiscordGuildThreadMembersUpdated(ThreadMembersUpdatedEvent members, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadMembersUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadMembersUpdated = nameof(OnDiscordGuildThreadMembersUpdated);
        
        /// <code>
        /// void OnDiscordUnhandledCommand(EventPayload payload)
        /// {
        ///     Puts("OnDiscordUnhandledCommand Works!");
        /// }
        /// </code>
        public const string OnDiscordUnhandledCommand = nameof(OnDiscordUnhandledCommand);        
        
        /// <code>
        /// void OnDiscordStageInstanceCreated(StageInstance stage, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordStageInstanceCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordStageInstanceCreated = nameof(OnDiscordStageInstanceCreated);
        
        /// <code>
        /// void OnDiscordStageInstanceUpdated(StageInstance stage, StageInstance previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordStageInstanceUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordStageInstanceUpdated = nameof(OnDiscordStageInstanceUpdated);
        
        /// <code>
        /// void OnDiscordStageInstanceDeleted(StageInstance stage, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordStageInstanceDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordStageInstanceDeleted = nameof(OnDiscordStageInstanceDeleted);
        #endregion
    }
    
    /// <summary>
    /// Represents all hooks available in the discord extension
    /// </summary>
    public static class DiscordExtHooks
    {
        #region Bot Client Hooks
        /// <code>
        /// void OnDiscordClientConnected(Plugin owner, DiscordClient client)
        /// {
        ///     Puts("OnDiscordClientConnected Works!");
        /// }
        /// </code>
        public const string OnDiscordClientConnected = nameof(OnDiscordClientConnected);
        
        /// <code>
        /// void OnDiscordClientDisconnected(Plugin owner, DiscordClient client)
        /// {
        ///     Puts("OnDiscordClientDisconnected Works!");
        /// }
        /// </code>
        public const string OnDiscordClientDisconnected = nameof(OnDiscordClientDisconnected);
        
        /// <code>
        /// void OnDiscordClientCreated()
        /// {
        ///     Puts("OnDiscordClientCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordClientCreated = nameof(OnDiscordClientCreated);
        #endregion

        #region Socket Hooks
        /// <code>
        /// void OnDiscordWebsocketOpened()
        /// {
        ///     Puts("OnDiscordWebsocketOpened Works!");
        /// }
        /// </code>
        public const string OnDiscordWebsocketOpened = nameof(OnDiscordWebsocketOpened);
        
        /// <code>
        /// void OnDiscordWebsocketClosed(string reason, ushort code, bool wasClean)
        /// {
        ///     Puts("OnDiscordWebsocketClosed Works!");
        /// }
        /// </code>
        public const string OnDiscordWebsocketClosed = nameof(OnDiscordWebsocketClosed);
        
        /// <code>
        /// void OnDiscordWebsocketErrored(Exception ex, string message)
        /// {
        ///     Puts("OnDiscordWebsocketErrored Works!");
        /// }
        /// </code>
        public const string OnDiscordWebsocketErrored = nameof(OnDiscordWebsocketErrored);
        
        /// <code>
        /// void OnDiscordSetupHeartbeat(float heartbeat)
        /// {
        ///     Puts("OnDiscordHeartbeatSent Works!");
        /// }
        /// </code>
        public const string OnDiscordSetupHeartbeat = nameof(OnDiscordSetupHeartbeat);
        
        /// <code>
        /// void OnDiscordHeartbeatSent()
        /// {
        ///     Puts("OnDiscordHeartbeatSent Works!");
        /// }
        /// </code>
        public const string OnDiscordHeartbeatSent = nameof(OnDiscordHeartbeatSent);
        #endregion

        #region Link Hooks
        /// <code>
        /// void OnDiscordPlayerLinked(IPlayer player, DiscordUser discord)
        /// {
        ///     Puts("OnDiscordPlayerLinked Works!");
        /// }
        /// </code>
        public const string OnDiscordPlayerLinked = nameof(OnDiscordPlayerLinked);
        
        /// <code>
        /// void OnDiscordPlayerUnlinked(IPlayer player, DiscordUser discord)
        /// {
        ///     Puts("OnDiscordPlayerUnlinked Works!");
        /// }
        /// </code>
        public const string OnDiscordPlayerUnlinked = nameof(OnDiscordPlayerUnlinked);
        #endregion

        #region Discord Event Hooks
        /// <code>
        /// void OnDiscordGatewayReady(GatewayReadyEvent ready)
        /// {
        ///     Puts("OnDiscordGatewayReady Works!");
        /// }
        /// </code>
        public const string OnDiscordGatewayReady = nameof(OnDiscordGatewayReady);
        
        /// <code>
        /// void OnDiscordGatewayResumed(GatewayResumedEvent resume)
        /// {
        ///     Puts("OnDiscordGatewayResumed Works!");
        /// }
        /// </code>
        public const string OnDiscordGatewayResumed = nameof(OnDiscordGatewayResumed);
        
        /// <code>
        /// void OnDiscordDirectChannelCreated(Channel channel)
        /// {
        ///     Puts("OnDiscordDirectChannelCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelCreated = nameof(OnDiscordDirectChannelCreated);
        
        /// <code>
        /// void OnDiscordGuildChannelCreated(Channel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelCreated = nameof(OnDiscordGuildChannelCreated);
        
        /// <code>
        /// Note: previous will be null if previous channel not found
        /// void OnDiscordDirectChannelUpdated(Channel channel, DiscordChannel previous)
        /// {
        ///     Puts("OnDiscordDirectChannelUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelUpdated = nameof(OnDiscordDirectChannelUpdated);
        
        /// <code>
        /// void OnDiscordGuildChannelUpdated(DiscordChannel channel, DiscordChannel previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelUpdated = nameof(OnDiscordGuildChannelUpdated);
        
        /// <code>
        /// Note: Not sure if this will ever happen but we handle it if it does
        /// void OnDiscordDirectChannelDeleted(DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectChannelDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelDeleted = nameof(OnDiscordDirectChannelDeleted);
        
        /// <code>
        /// void OnDiscordGuildChannelDeleted(DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelDeleted = nameof(OnDiscordGuildChannelDeleted);
        
        /// <code>
        /// Note: Channel will be null if we haven't seen it yet
        /// void OnDiscordDirectChannelPinsUpdated(ChannelPinsUpdatedEvent update, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectChannelPinsUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectChannelPinsUpdated = nameof(OnDiscordDirectChannelPinsUpdated);
        
        /// <code>
        /// void OnDiscordGuildChannelPinsUpdated(ChannelPinsUpdatedEvent update, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildChannelPinsUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildChannelPinsUpdated = nameof(OnDiscordGuildChannelPinsUpdated);
        
        /// <code>
        /// void OnDiscordGuildCreated(GuildDiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildCreated = nameof(OnDiscordGuildCreated);
        
        /// <code>
        /// Note: previous will be null if guild previously not loaded
        /// void OnDiscordGuildUpdated(DiscordGuild guild, DiscordGuild previous)
        /// {
        ///     Puts("OnDiscordGuildUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildUpdated = nameof(OnDiscordGuildUpdated);
        
        /// <code>
        /// void OnDiscordGuildUnavailable(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildUnavailable Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildUnavailable = nameof(OnDiscordGuildUnavailable);
        
        /// <code>
        /// void OnDiscordGuildDeleted(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildDeleted = nameof(OnDiscordGuildDeleted);
        
        /// <code>
        /// void OnDiscordGuildMemberBanned(GuildMemberBannedEvent ban, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberBanned Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberBanned = nameof(OnDiscordGuildMemberBanned);
        
        /// <code>
        /// void OnDiscordGuildMemberUnbanned(GuildMemberBannedEvent ban, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildBanRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberUnbanned = nameof(OnDiscordGuildMemberUnbanned);
        
        /// <code>
        /// void OnDiscordGuildEmojisUpdated(GuildEmojisUpdatedEvent emojis, Hash&lt;Snowflake, DiscordEmoji&gt; previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildEmojisUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildEmojisUpdated = nameof(OnDiscordGuildEmojisUpdated);        
        
        /// <code>
        /// void OnDiscordGuildStickersUpdated(GuildStickersUpdatedEvent stickers, Hash&lt;Snowflake, DiscordSticker&gt; previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildStickersUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildStickersUpdated = nameof(OnDiscordGuildStickersUpdated);
        
        /// <code>
        /// void OnDiscordGuildIntegrationsUpdated(GuildIntegrationsUpdatedEvent integration, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildIntegrationsUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildIntegrationsUpdated = nameof(OnDiscordGuildIntegrationsUpdated);
        
        /// <code>
        /// void OnDiscordGuildMemberAdded(GuildMemberAddedEvent member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberAdded = nameof(OnDiscordGuildMemberAdded);
        
        /// <code>
        /// void OnDiscordGuildMemberRemoved(GuildMemberRemovedEvent member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberRemoved = nameof(OnDiscordGuildMemberRemoved);
        
        /// <code>
        /// void OnDiscordGuildMemberUpdated(GuildMemberUpdatedEvent member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberUpdated = nameof(OnDiscordGuildMemberUpdated);
        
        /// <code>
        /// void OnDiscordGuildMembersLoaded(DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMembersLoaded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMembersLoaded = nameof(OnDiscordGuildMembersLoaded);
        
        /// <code>
        /// void OnDiscordGuildMembersChunk(GuildMembersChunkEvent chunk, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMembersChunk Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMembersChunk = nameof(OnDiscordGuildMembersChunk);
        
        /// <code>
        /// void OnDiscordGuildRoleCreated(DiscordRole role, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildRoleCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildRoleCreated = nameof(OnDiscordGuildRoleCreated);
        
        /// <code>
        /// void OnDiscordGuildRoleUpdated(DiscordRole role, DiscordRole previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildRoleUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildRoleUpdated = nameof(OnDiscordGuildRoleUpdated);
        
        /// <code>
        /// void OnDiscordGuildRoleDeleted(DiscordRole role, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildRoleDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildRoleDeleted = nameof(OnDiscordGuildRoleDeleted);
        
        /// <code>
        /// void OnDiscordGuildScheduledEventCreated(GuildScheduledEvent guildEvent, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildScheduledEventCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildScheduledEventCreated = nameof(OnDiscordGuildScheduledEventCreated);
        
        /// <code>
        /// void OnDiscordGuildScheduledEventUpdated(GuildScheduledEvent guildEvent, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildScheduledEventUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildScheduledEventUpdated = nameof(OnDiscordGuildScheduledEventUpdated);
        
        /// <code>
        /// void OnDiscordGuildScheduledEventDeleted(GuildScheduledEvent guildEvent, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildScheduledEventDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildScheduledEventDeleted = nameof(OnDiscordGuildScheduledEventDeleted);
        
        /// <code>
        /// void OnDiscordGuildScheduledEventUserAdded(GuildScheduleEventUserAddedEvent added, GuildScheduledEvent, scheduledEvent, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildScheduledEventUserAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildScheduledEventUserAdded = nameof(OnDiscordGuildScheduledEventUserAdded);
        
        /// <code>
        /// void OnDiscordGuildScheduledEventUserRemoved(GuildScheduleEventUserRemovedEvent removed, GuildScheduledEvent, scheduledEvent, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildScheduledEventUserRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildScheduledEventUserRemoved = nameof(OnDiscordGuildScheduledEventUserRemoved);
        
        /// <code>
        /// void OnDiscordCommand(DiscordMessage message, string cmd, string[] args)
        /// {
        ///     Puts("OnDiscordCommand Works!");
        /// }
        /// </code>
        public const string OnDiscordCommand = nameof(OnDiscordCommand);
        
        /// <code>
        /// Note: Channel may be null if we haven't seen it yet
        /// void OnDiscordDirectMessageCreated(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageCreated = nameof(OnDiscordDirectMessageCreated);
        
        /// <code>
        /// void OnDiscordGuildMessageCreated(DiscordMessage message, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageCreated = nameof(OnDiscordGuildMessageCreated);
        
        /// <code>
        /// void OnDiscordDirectMessageUpdated(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageUpdated = nameof(OnDiscordDirectMessageUpdated);
        
        /// <code>
        /// void OnDiscordDirectMessageUpdated(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageUpdated = nameof(OnDiscordGuildMessageUpdated);
        
        /// <code>
        /// void OnDiscordDirectMessageDeleted(DiscordMessage message, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageDeleted = nameof(OnDiscordDirectMessageDeleted);
        
        /// <code>
        /// void OnDiscordDirectMessageDeleted(DiscordMessage message, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordDirectMessageDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageDeleted = nameof(OnDiscordGuildMessageDeleted);
        
        /// <code>
        /// void OnDiscordDirectMessagesBulkDeleted(List&lt;Snowflake&gt; messageIds, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessagesBulkDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessagesBulkDeleted = nameof(OnDiscordDirectMessagesBulkDeleted);
        
        /// <code>
        /// void OnDiscordGuildMessagesBulkDeleted(List&lt;Snowflake&gt; messageIds, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessagesBulkDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessagesBulkDeleted = nameof(OnDiscordDirectMessagesBulkDeleted);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionAdded(MessageReactionAddedEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionAdded = nameof(OnDiscordDirectMessageReactionAdded);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionAdded(MessageReactionAddedEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionAdded Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionAdded = nameof(OnDiscordGuildMessageReactionAdded);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionRemoved(MessageReactionRemovedEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionRemoved = nameof(OnDiscordDirectMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionRemoved(MessageReactionRemovedEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionRemoved = nameof(OnDiscordGuildMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionRemovedAll(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionRemovedAll Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionRemovedAll = nameof(OnDiscordDirectMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionRemovedAll(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionRemovedAll Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionRemovedAll = nameof(OnDiscordGuildMessageReactionRemoved);
        
        /// <code>
        /// void OnDiscordDirectMessageReactionEmojiRemoved(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectMessageReactionEmojiRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectMessageReactionEmojiRemoved = nameof(OnDiscordDirectMessageReactionEmojiRemoved);
        
        /// <code>
        /// void OnDiscordGuildMessageReactionEmojiRemoved(MessageReactionRemovedAllEmojiEvent reaction, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMessageReactionEmojiRemoved Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMessageReactionEmojiRemoved = nameof(OnDiscordGuildMessageReactionEmojiRemoved);
        
        /// <code>
        /// void OnDiscordGuildMemberPresenceUpdated(PresenceUpdatedEvent update, GuildMember member, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildMemberPresenceUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildMemberPresenceUpdated = nameof(OnDiscordGuildMemberPresenceUpdated);
        
        /// <code>
        /// void OnDiscordDirectTypingStarted(TypingStartedEvent typing, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectTypingStarted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectTypingStarted = nameof(OnDiscordDirectTypingStarted);
        
        /// <code>
        /// void OnDiscordGuildTypingStarted(TypingStartedEvent typing, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildTypingStarted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildTypingStarted = nameof(OnDiscordGuildTypingStarted);
        
        /// <code>
        /// void OnDiscordUserUpdated(DiscordUser user)
        /// {
        ///     Puts("OnDiscordUserUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordUserUpdated = nameof(OnDiscordUserUpdated);
        
        /// <code>
        /// void OnDiscordDirectVoiceStateUpdated(VoiceState voice, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectVoiceStateUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectVoiceStateUpdated = nameof(OnDiscordDirectVoiceStateUpdated);
        
        /// <code>
        /// void OnDiscordGuildVoiceStateUpdated(VoiceState voice, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildVoiceStateUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildVoiceStateUpdated = nameof(OnDiscordGuildVoiceStateUpdated);
        
        /// <code>
        /// void OnDiscordGuildVoiceServerUpdated(VoiceServerUpdatedEvent voice, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildVoiceServerUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildVoiceServerUpdated = nameof(OnDiscordGuildVoiceServerUpdated);
        
        /// <code>
        /// void OnDiscordGuildWebhookUpdated(WebhooksUpdatedEvent webhook, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildWebhookUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildWebhookUpdated = nameof(OnDiscordGuildWebhookUpdated);
        
        /// <code>
        /// void OnDiscordDirectInviteCreated(InviteCreatedEvent invite, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectInviteCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectInviteCreated = nameof(OnDiscordDirectInviteCreated);
        
        /// <code>
        /// void OnDiscordGuildInviteCreated(InviteCreatedEvent invite, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildInviteCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildInviteCreated = nameof(OnDiscordGuildInviteCreated);
        
        /// <code>
        /// void OnDiscordDirectInviteDeleted(InviteCreatedEvent invite, DiscordChannel channel)
        /// {
        ///     Puts("OnDiscordDirectInviteDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordDirectInviteDeleted = nameof(OnDiscordDirectInviteDeleted);
        
        /// <code>
        /// void OnDiscordGuildInviteDeleted(InviteCreatedEvent invite, DiscordChannel channel, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildInviteDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildInviteDeleted = nameof(OnDiscordGuildInviteDeleted);
        
        /// <code>
        /// void OnDiscordInteractionCreated(DiscordInteraction interaction)
        /// {
        ///     Puts("OnDiscordInteractionCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordInteractionCreated = nameof(OnDiscordInteractionCreated);
        
        /// <code>
        /// void OnDiscordGuildIntegrationCreated(IntegrationCreatedEvent integration, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildIntegrationCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildIntegrationCreated = nameof(OnDiscordGuildIntegrationCreated);
        
        /// <code>
        /// void OnDiscordGuildIntegrationUpdated(IntegrationUpdatedEvent interaction, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildIntegrationUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildIntegrationUpdated = nameof(OnDiscordGuildIntegrationUpdated);
        
        /// <code>
        /// void OnDiscordIntegrationDeleted(IntegrationDeletedEvent interaction, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordIntegrationDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordIntegrationDeleted = nameof(OnDiscordIntegrationDeleted);

        /// <code>
        /// void OnDiscordGuildThreadCreated(DiscordChannel thread, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadCreated = nameof(OnDiscordGuildThreadCreated);
        
        /// <code>
        /// void OnDiscordGuildThreadUpdated(DiscordChannel thread, DiscordChannel previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadUpdated = nameof(OnDiscordGuildThreadUpdated);

        /// <code>
        /// void OnDiscordGuildThreadDeleted(DiscordChannel thread, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadDeleted = nameof(OnDiscordGuildThreadDeleted);
        
        /// <code>
        /// void OnDiscordGuildThreadListSynced(ThreadListSyncEvent sync, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadListSynced Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadListSynced = nameof(OnDiscordGuildThreadListSynced);
        
        /// <code>
        /// void OnDiscordGuildThreadMemberUpdated(ThreadMember member, DiscordChannel thread, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadMemberUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadMemberUpdated = nameof(OnDiscordGuildThreadMemberUpdated);
        
        /// <code>
        /// void OnDiscordGuildThreadMembersUpdated(ThreadMembersUpdatedEvent members, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordGuildThreadMembersUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordGuildThreadMembersUpdated = nameof(OnDiscordGuildThreadMembersUpdated);
        
        /// <code>
        /// void OnDiscordUnhandledCommand(EventPayload payload)
        /// {
        ///     Puts("OnDiscordUnhandledCommand Works!");
        /// }
        /// </code>
        public const string OnDiscordUnhandledCommand = nameof(OnDiscordUnhandledCommand);        
        
        /// <code>
        /// void OnDiscordStageInstanceCreated(StageInstance stage, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordStageInstanceCreated Works!");
        /// }
        /// </code>
        public const string OnDiscordStageInstanceCreated = nameof(OnDiscordStageInstanceCreated);
        
        /// <code>
        /// void OnDiscordStageInstanceUpdated(StageInstance stage, StageInstance previous, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordStageInstanceUpdated Works!");
        /// }
        /// </code>
        public const string OnDiscordStageInstanceUpdated = nameof(OnDiscordStageInstanceUpdated);
        
        /// <code>
        /// void OnDiscordStageInstanceDeleted(StageInstance stage, DiscordGuild guild)
        /// {
        ///     Puts("OnDiscordStageInstanceDeleted Works!");
        /// }
        /// </code>
        public const string OnDiscordStageInstanceDeleted = nameof(OnDiscordStageInstanceDeleted);
        #endregion
    }
}