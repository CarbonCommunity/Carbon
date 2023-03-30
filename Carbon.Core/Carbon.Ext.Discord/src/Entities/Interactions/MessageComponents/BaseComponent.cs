using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/message-components#component-object">Message Component</a> within discord
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BaseComponent
    {
        /// <summary>
        /// Message component type
        /// </summary>
        [JsonProperty("type")]
        public MessageComponentType Type { get; protected set; }
    }
}