using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels.Threads;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#thread-member-update">Thread Member Update Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThreadMemberUpdateEvent : ThreadMember
    {
        /// <summary>
        /// The ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }   
    }
}