using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#typing-start">Typing Start</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TypingStartedEvent
    {
        /// <summary>
        /// ID of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }
        
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }
        
        /// <summary>
        /// ID of the user
        /// </summary>
        [JsonProperty("user_id")]
        public Snowflake UserId { get; set; }

        /// <summary>
        /// Unix time (in seconds) of when the user started typing
        /// </summary>
        [JsonProperty("timestamp")]
        public int? Timestamp { get; set; }
        
        /// <summary>
        /// The member who started typing if this happened in a guild
        /// </summary>
        [JsonProperty("member")]
        public GuildMember Member { get; set; }
    }
}
