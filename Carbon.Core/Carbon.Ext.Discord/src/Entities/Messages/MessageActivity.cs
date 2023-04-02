using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#message-object-message-activity-structure">Message Activity Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MessageActivity
    {
        /// <summary>
        /// Type of message activity
        /// <see cref="MessageActivityType"/>
        /// </summary>
        [JsonProperty("type")]
        public MessageActivityType Type { get; set; }
        
        /// <summary>
        /// Party ID from a Rich Presence event
        /// </summary>
        [JsonProperty("party_id")]
        public string PartyId { get; set; }
    }
}