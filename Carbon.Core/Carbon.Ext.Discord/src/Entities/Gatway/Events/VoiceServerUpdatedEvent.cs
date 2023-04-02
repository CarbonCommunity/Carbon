using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#voice-server-update">Voice Server Update</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class VoiceServerUpdatedEvent
    {
        /// <summary>
        /// Voice connection token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// The guild this voice server update is for
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// The voice server host
        /// </summary>
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
    }
}
