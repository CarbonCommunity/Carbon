using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#modify-guild-welcome-screen"></a>
    /// </summary>
    public class WelcomeScreenUpdate
    {
        /// <summary>
        /// Whether the welcome screen is enabled
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        
        /// <summary>
        /// Channels linked in the welcome screen and their display options
        /// Up to 5
        /// </summary>
        [JsonProperty("welcome_channels")]
        public List<GuildWelcomeScreenChannel> WelcomeChannels { get; set; }
        
        /// <summary>
        /// The server description shown in the welcome screen
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}