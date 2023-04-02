using Newtonsoft.Json;
namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/topics/gateway#guild-scheduled-event-user-add-guild-scheduled-event-user-add-event-fields">Guild Scheduled Event User Add Event Fields</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildScheduleEventUserAddedEvent
    {
        /// <summary>
        /// ID of the guild scheduled event
        /// </summary>
        [JsonProperty("guild_scheduled_event_id")]
        public Snowflake GuildScheduledEventId { get; set; }
        
        /// <summary>
        /// ID of the user
        /// </summary>
        [JsonProperty("user_id")]
        public Snowflake UserId { get; set; }
        
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
    }
}