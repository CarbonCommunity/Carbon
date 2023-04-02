using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Interfaces;
namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild-scheduled-event">Guild Scheduled Event Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildScheduledEvent : ISnowflakeEntity
    {
        /// <summary>
        /// The ID of the scheduled event
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The guild ID which the scheduled event belongs to
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
        
        /// <summary>
        /// The channel ID in which the scheduled event will be hosted, or null if <see cref="ScheduledEventEntityType">scheduled entity type</see> is <see cref="ScheduledEventEntityType.External">EXTERNAL</see>
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake? ChannelId  { get; set; }
        
        /// <summary>
        /// The ID of the user that created the scheduled event
        /// </summary>
        [JsonProperty("creator_id")]
        public Snowflake? CreatorId  { get; set; }
        
        /// <summary>
        /// The name of the scheduled event (1-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The description of the scheduled event (1-1000 characters)
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The time the scheduled event will start
        /// </summary>
        [JsonProperty("scheduled_start_time")]
        public DateTime ScheduledStartTime { get; set; }
        
        /// <summary>
        /// The time the scheduled event will end, required if <see cref="GuildScheduledEvent.EntityType">EntityType</see> is <see cref="ScheduledEventEntityType.External">EXTERNAL</see>
        /// </summary>
        [JsonProperty("scheduled_end_time ")]
        public DateTime? ScheduledEndTime { get; set; }
        
        /// <summary>
        /// The privacy level of the scheduled event
        /// </summary>
        [JsonProperty("privacy_level")]
        public ScheduledEventPrivacyLevel PrivacyLevel { get; set; }
        
        /// <summary>
        /// The status of the scheduled event
        /// </summary>
        [JsonProperty("status")]
        public ScheduledEventStatus Status { get; set; }
        
        /// <summary>
        /// The type of the scheduled event
        /// </summary>
        [JsonProperty("entity_type")]
        public ScheduledEventEntityType EntityType { get; set; }
        
        /// <summary>
        /// The id of an entity associated with a guild scheduled event
        /// </summary>
        [JsonProperty("entity_id")]
        public Snowflake? EntityId  { get; set; }
        
        /// <summary>
        /// Additional metadata for the guild scheduled event
        /// </summary>
        [JsonProperty("entity_metadata")]
        public ScheduledEventEntityMetadata EntityMetadata { get; set; }
        
        /// <summary>
        /// The user that created the scheduled event
        /// </summary>
        [JsonProperty("creator")]
        public DiscordUser Creator { get; set; }
        
        /// <summary>
        /// The number of users subscribed to the scheduled event
        /// </summary>
        [JsonProperty("user_count")]
        public int? UserCount { get; set; }

        /// <summary>
        /// Returns a list of guild scheduled event objects for the given guild.
        /// See <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#list-scheduled-events-for-guild">List Scheduled Events for Guild</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild to get events for</param>
        /// <param name="lookup">Query string parameters</param>
        /// <param name="callback">Callback with the list of scheduled events</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void ListForGuild(DiscordClient client, Snowflake guildId, ScheduledEventLookup lookup = null, Action<List<GuildScheduledEvent>> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/scheduled-events{lookup?.ToQueryString()}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Create a guild scheduled event in the guild.
        /// Returns a <see cref="GuildScheduledEvent">guild scheduled event</see> object on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#create-guild-scheduled-event">Create Guild Scheduled Event</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild to create event in</param>
        /// <param name="create">Guild Scheduled Event to create</param>
        /// <param name="callback">Callback with the created scheduled events</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void Create(DiscordClient client, Snowflake guildId, ScheduledEventCreate create, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/scheduled-events", RequestMethod.POST, create, callback, error);
        }

        /// <summary>
        /// Get a guild scheduled event.
        /// Returns a guild scheduled event object on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#get-guild-scheduled-event">Get Guild Scheduled Event</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild to get events for</param>
        /// <param name="eventId">Id of the scheduled event</param>
        /// <param name="lookup">Query string parameters</param>
        /// <param name="callback">Callback with the scheduled event</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void Get(DiscordClient client, Snowflake guildId, Snowflake eventId, ScheduledEventLookup lookup = null, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            if (!eventId.IsValid()) throw new InvalidSnowflakeException(nameof(eventId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/scheduled-events/{eventId}{lookup?.ToQueryString()}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Modify a guild scheduled event.
        /// Returns the modified <see cref="GuildScheduledEvent">guild scheduled event</see> object on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#modify-guild-scheduled-event">Modify Guild Scheduled Event</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild to modify event in</param>
        /// <param name="eventId">Id of the event to update</param>
        /// <param name="update">Guild Scheduled Event to update</param>
        /// <param name="callback">Callback with the updated scheduled events</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void Modify(DiscordClient client, Snowflake guildId, Snowflake eventId, ScheduledEventUpdate update, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            if (!eventId.IsValid()) throw new InvalidSnowflakeException(nameof(eventId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/scheduled-events/{eventId}", RequestMethod.PATCH, update, callback, error);
        }

        /// <summary>
        /// Delete a guild scheduled event
        /// See <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#delete-guild-scheduled-event">Delete Guild Scheduled Event</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild to modify event in</param>
        /// <param name="eventId">Id of the event to delete</param>
        /// <param name="callback">Callback with the updated scheduled events</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void Delete(DiscordClient client, Snowflake guildId, Snowflake eventId, Action<GuildScheduledEvent> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            if (!eventId.IsValid()) throw new InvalidSnowflakeException(nameof(eventId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/scheduled-events/{eventId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Get a list of guild scheduled event users subscribed to a guild scheduled event.
        /// Returns a list of guild scheduled event user objects on success.
        /// Guild member data, if it exists, is included if the WithMember query parameter is set.
        /// See <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#get-guild-scheduled-event-users">Get Guild Scheduled Event Users</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild to get event users for</param>
        /// <param name="eventId">Id of the event to get users for</param>
        /// <param name="lookup">Query string parameters</param>
        /// <param name="callback">Callback with the list of scheduled event users</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetUsers(DiscordClient client, Snowflake guildId, Snowflake eventId, ScheduledEventUsersLookup lookup = null, Action<List<ScheduledEventUser>> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            if (!eventId.IsValid()) throw new InvalidSnowflakeException(nameof(eventId));
            
            if (lookup?.Limit != null && lookup.Limit.Value > 100)
            {
                throw new Exception($"{nameof(GuildScheduledEvent)}.{nameof(GetUsers)} Validation Error: {nameof(ScheduledEventUsersLookup)}.{nameof(ScheduledEventUsersLookup.Limit)} cannot be greater than 100");
            }
            
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/scheduled-events/{eventId}{lookup?.ToQueryString()}", RequestMethod.GET, null, callback, error);
        }
        
        internal void Update(GuildScheduledEvent scheduledEvent)
        {
            if (scheduledEvent.ChannelId.HasValue)
            {
                ChannelId = scheduledEvent.ChannelId;
            }
            
            if (scheduledEvent.EntityMetadata != null)
            {
                if (EntityMetadata == null)
                {
                    EntityMetadata = scheduledEvent.EntityMetadata;
                }
                else
                {
                    EntityMetadata.Update(scheduledEvent.EntityMetadata);
                }
            }

            if (scheduledEvent.Name != null)
            {
                Name = scheduledEvent.Name;
            }
            
                        
            if (scheduledEvent.Description != null)
            {
                Description = scheduledEvent.Description;
            }
            
            PrivacyLevel = scheduledEvent.PrivacyLevel;
            EntityType = scheduledEvent.EntityType;
            Status = scheduledEvent.Status;
            ScheduledStartTime = scheduledEvent.ScheduledStartTime;
            ScheduledEndTime = scheduledEvent.ScheduledEndTime;
        }
    }
}