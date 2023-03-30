using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#message-reaction-remove">Message Reaction Remove</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MessageReactionRemovedEvent
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        [JsonProperty("user_id")]
        public Snowflake UserId { get; set; }

        /// <summary>
        /// The id of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }

        /// <summary>
        /// The id of the message
        /// </summary>
        [JsonProperty("message_id")]
        public Snowflake MessageId { get; set; }
        
        /// <summary>
        /// The id of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }

        /// <summary>
        /// The emoji removed
        /// </summary>
        [JsonProperty("emoji")]
        public DiscordEmoji Emoji { get; set; }
    }
}