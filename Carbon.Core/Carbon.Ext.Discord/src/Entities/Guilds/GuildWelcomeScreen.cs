using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#welcome-screen-object">Welcome Screen Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildWelcomeScreen
    {
        /// <summary>
        /// The server description shown in the welcome screen
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The channels shown in the welcome screen
        /// Up to 5
        /// </summary>
        [JsonProperty("welcome_channels")]
        public List<GuildWelcomeScreenChannel> WelcomeChannels { get; set; }
    }
}