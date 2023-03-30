using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#message-object-message-reference-structure">Message Reference Structure</a> for a message
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MessageReference
    {
        /// <summary>
        /// ID of the originating message
        /// </summary>
        [JsonProperty("message_id")]
        public Snowflake MessageId { get; set; }
        
        /// <summary>
        /// ID of the originating message's channel
        /// Is optional when creating a reply, but will always be present when receiving an event/response that includes this data model.
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake? ChannelId { get; set; }
        
        /// <summary>
        /// ID of the originating message's guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }

        /// <summary>
        /// When sending, whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message, default true
        /// </summary>
        [JsonProperty("fail_if_not_exists")]
        public bool? FailIfNotExists { get; set; }
    }
}