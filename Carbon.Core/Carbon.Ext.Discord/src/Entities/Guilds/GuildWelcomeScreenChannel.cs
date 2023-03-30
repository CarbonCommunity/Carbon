using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-channel-structure">Welcome Screen Channel Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildWelcomeScreenChannel
    {
        /// <summary>
        /// Channel ID for the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }
        
        /// <summary>
        /// The description shown for the channel
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The emoji id, if the emoji is custom
        /// </summary>
        [JsonProperty("emoji_id")]
        public Snowflake EmojiId { get; set; }
        
        /// <summary>
        /// The emoji name if custom, the unicode character if standard, or null if no emoji is set
        /// </summary>
        [JsonProperty("emoji_name")]
        public string EmojiName { get; set; }
    }
}