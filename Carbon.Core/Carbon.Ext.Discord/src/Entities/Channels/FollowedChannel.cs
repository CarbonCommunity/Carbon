using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#followed-channel-object-followed-channel-structure">Followed Channel Structure</a> from an API response
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class FollowedChannel
    {
        /// <summary>
        /// Source channel ID
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }
        
        /// <summary>
        /// Created target webhook ID
        /// </summary>
        [JsonProperty("webhook_id")]
        public Snowflake WebhookId { get; set; }
    }
}