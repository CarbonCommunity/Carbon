using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#client-status-object">Client Status Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ClientStatus
    {
        /// <summary>
        /// The user's status set for an active desktop (Windows, Linux, Mac) application session
        /// </summary>
        [JsonProperty("desktop")]
        public string Desktop { get; set; }
        
        /// <summary>
        /// The user's status set for an active mobile (iOS, Android) application session
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
        
        /// <summary>
        /// The user's status set for an active web (browser, bot account) application session
        /// </summary>
        [JsonProperty("web")]
        public string Web { get; set; }
    }
}