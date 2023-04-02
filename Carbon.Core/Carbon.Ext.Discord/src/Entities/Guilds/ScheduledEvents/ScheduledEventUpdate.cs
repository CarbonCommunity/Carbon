using System;
using Newtonsoft.Json;
namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#modify-guild-scheduled-event">Guild Scheduled Event Update</a> within discord
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ScheduledEventUpdate
    {
        /// <summary>
        /// The channel ID in which the scheduled event will be hosted, or null if <see cref="ScheduledEventEntityType">scheduled entity type</see> is <see cref="ScheduledEventEntityType.External">External</see>
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake? ChannelId  { get; set; }
        
        /// <summary>
        /// Additional metadata for the guild scheduled event
        /// </summary>
        [JsonProperty("entity_metadata")]
        public ScheduledEventEntityMetadata EntityMetadata { get; set; }
        
        /// <summary>
        /// The name of the scheduled event (1-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The privacy level of the scheduled event
        /// </summary>
        [JsonProperty("privacy_level")]
        public ScheduledEventPrivacyLevel? PrivacyLevel { get; set; }
        
        /// <summary>
        /// The time the scheduled event will start
        /// </summary>
        [JsonProperty("scheduled_start_time")]
        public DateTime? ScheduledStartTime { get; set; }
        
        /// <summary>
        /// The time the scheduled event will end, required if <see cref="GuildScheduledEvent.EntityType">EntityType</see> is <see cref="ScheduledEventEntityType.External">EXTERNAL</see>
        /// </summary>
        [JsonProperty("scheduled_end_time ")]
        public DateTime? ScheduledEndTime { get; set; }
        
        /// <summary>
        /// The description of the scheduled event (1-1000 characters)
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The type of the scheduled event
        /// </summary>
        [JsonProperty("entity_type")]
        public ScheduledEventEntityType? EntityType { get; set; }
        
        /// <summary>
        /// The status of the scheduled event
        /// </summary>
        [JsonProperty("status")]
        public ScheduledEventStatus? Status { get; set; }
    }
}