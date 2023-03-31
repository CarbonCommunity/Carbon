using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Integrations
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#integration-application-object">Integration Application Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IntegrationApplication
    {
        /// <summary>
        /// The ID of the app
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The name of the app
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The icon hash of the app
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
        
        /// <summary>
        /// The description of the app
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The summary of the app
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }
        
        /// <summary>
        /// The bot associated with this application
        /// </summary>
        [JsonProperty("bot")]
        public DiscordUser Bot { get; set; }
    }
}