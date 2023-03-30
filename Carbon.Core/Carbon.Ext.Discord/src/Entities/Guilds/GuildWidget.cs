using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href=""></a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildWidget
    {
        /// <summary>
        /// Guild id
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// Guild name (2-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Instant invite for the guilds specified widget invite channel
        /// </summary>
        [JsonProperty("instant_invite")]
        public string InstantInvite { get; set; }
        
        /// <summary>
        /// Voice and stage channels which are accessible by @everyone
        /// </summary>
        [JsonProperty("channels")]
        public List<DiscordChannel> Channels { get; set; }
        
        /// <summary>
        /// Special widget user objects that includes users presence (Limit 100)
        /// </summary>
        [JsonProperty("members")]
        public List<DiscordUser> Members { get; set; }
        
        /// <summary>
        /// Number of online members in this guild
        /// </summary>
        [JsonProperty("presence_count")]
        public int PresenceCount { get; set; }
    }
}