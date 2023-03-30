using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Oxide.Ext.Discord.Entities.Api
{
    /// <summary>
    /// Represents and error from the discord API
    /// </summary>
    public class DiscordApiError
    {
        /// <summary>
        /// Error code from the discord API
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }
        
        /// <summary>
        /// Error message from the discord API
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
        
        /// <summary>
        /// An JObject containing the errors that occured
        /// </summary>
        [JsonProperty("errors")]
        public JObject Errors { get; set; }
    }
}