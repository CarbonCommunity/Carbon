using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Configuration
{
    /// <summary>
    /// Represents discord extension command configuration
    /// </summary>
    public class DiscordCommandsConfig
    {
        /// <summary>
        /// Array of command prefixes for discord commands
        /// </summary>
        [JsonProperty("Command Prefixes")]
        public char[] CommandPrefixes { get; set; }
    }
}