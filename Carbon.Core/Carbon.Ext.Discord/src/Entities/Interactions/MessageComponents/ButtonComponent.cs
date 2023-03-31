using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/interactions/message-components#buttons">Button Component</a> within discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ButtonComponent : BaseInteractableComponent
    {
        /// <summary>
        /// Style for the component
        /// </summary>
        [JsonProperty("style")]
        public ButtonStyle Style { get; set; }

        /// <summary>
        /// Text that appears on the button
        /// Max 80 characters
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Emoji on the component
        /// </summary>
        [JsonProperty("emoji")]
        public DiscordEmoji Emoji { get; set; }

        /// <summary>
        /// A url for link-style buttons
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// whether the button is disabled
        /// Default false
        /// </summary>
        [JsonProperty("disabled")]
        public bool? Disabled { get; set; }

        /// <summary>
        /// Constructor for button
        /// Sets type to button
        /// </summary>
        public ButtonComponent()
        {
            Type = MessageComponentType.Button;
        }
    }
}