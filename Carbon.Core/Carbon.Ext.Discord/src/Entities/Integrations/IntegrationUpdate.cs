using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Integrations
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#modify-guild-integration-json-params">Integration Update Structure</a> 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IntegrationUpdate
    {
        /// <summary>
        /// Whether emoticons should be synced for this integration (twitch only currently)
        /// </summary>
        [JsonProperty("enable_emoticons")]
        public bool? EnableEmoticons { get; set; } 

        /// <summary>
        /// The behavior when an integration subscription lapses
        /// </summary>
        [JsonProperty("expire_behaviour")]
        public IntegrationExpireBehaviors? ExpireBehaviour { get; set; }

        /// <summary>
        /// Period (in days) where the integration will ignore lapsed subscriptions
        /// </summary>
        [JsonProperty("expire_grace_period")]
        public int? ExpireGracePeriod { get; set; }
    }
}