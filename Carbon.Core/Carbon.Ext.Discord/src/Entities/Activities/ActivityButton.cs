using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Activities
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-buttons">Activity Buttons</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ActivityButton
    {
        /// <summary>
        /// The text shown on the button (1-32 characters)
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }
        
        /// <summary>
        /// The url opened when clicking the button (1-512 characters)
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}