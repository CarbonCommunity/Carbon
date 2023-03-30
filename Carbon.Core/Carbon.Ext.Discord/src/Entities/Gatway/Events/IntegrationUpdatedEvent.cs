using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Integrations;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/topics/gateway#integration-update-integration-update-event-additional-fields">Integration Update Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IntegrationUpdatedEvent : Integration
    {
        /// <summary>
        /// Guild ID the integration was updated In
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
    }
}