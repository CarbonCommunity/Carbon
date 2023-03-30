using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;
namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-user-object-guild-scheduled-event-user-structure">Guild Scheduled Event User Object</a> within discord
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ScheduledEventUser
    {
        /// <summary>
        /// The scheduled event id which the user subscribed to
        /// </summary>
        [JsonProperty("guild_scheduled_event_id")]
        public Snowflake GuildScheduledEventId { get; set; }
        
        /// <summary>
        /// User which subscribed to an event
        /// </summary>
        [JsonProperty("guild_scheduled_event_id")]
        public DiscordUser User { get; set; }
        
        /// <summary>
        /// Guild member data for this user for the guild which this event belongs to, if any
        /// </summary>
        [JsonProperty("member")]
        public GuildMember Member { get; set; }
    }
}