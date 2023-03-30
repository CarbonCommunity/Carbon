using Newtonsoft.Json;
namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/interactions/message-components#input-text">Input Text Component</a> within discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InputTextComponent : BaseInteractableComponent
    {
        /// <summary>
        /// The style of the input text
        /// </summary>
        [JsonProperty("style")]
        public InputTextStyles Style { get; set; }
        
        /// <summary>
        /// Text that appears on top of the input text field, max 80 characters
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }
        
        /// <summary>
        /// The minimum length of the text input
        /// </summary>
        [JsonProperty("min_length")]
        public int? MinLength { get; set; }
        
        /// <summary>
        /// The maximum length of the text input
        /// </summary>
        [JsonProperty("max_length")]
        public int? MaxLength { get; set; }
        
        /// <summary>
        /// The placeholder for the text input field
        /// </summary>
        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }
        
        /// <summary>
        /// The pre-filled value for text input
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        
        /// <summary>
        /// Is the Input Text Required to be filled out
        /// </summary>
        [JsonProperty("required")]
        public bool? Required { get; set; }
        
        /// <summary>
        /// Input Text Constructor
        /// </summary>
        public InputTextComponent()
        {
            Type = MessageComponentType.InputText;
        }
    }
}