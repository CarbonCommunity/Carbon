using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/topics/gateway#integration-delete-integration-delete-event-fields">Integration Delete Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IntegrationDeletedEvent
    {
        /// <summary>
        /// ID of the integration
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// Guild ID the integration was in
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
        
        /// <summary>
        /// Application ID of the integration
        /// </summary>
        [JsonProperty("application_id")]
        public Snowflake ApplicationId { get; set; }
    }
}