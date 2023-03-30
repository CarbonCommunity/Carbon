using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Activities
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-secrets">Activity Secrets</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ActivitySecrets
    {
        /// <summary>
        /// The secret for joining a party
        /// </summary>
        [JsonProperty("join")]
        public string Join { get; set; }
        
        /// <summary>
        /// The secret for spectating a game
        /// </summary>
        [JsonProperty("spectate")]
        public string Spectate { get; set; }
        
        /// <summary>
        /// The secret for a specific instanced match
        /// </summary>
        [JsonProperty("match")]
        public string Match { get; set; }
    }
}