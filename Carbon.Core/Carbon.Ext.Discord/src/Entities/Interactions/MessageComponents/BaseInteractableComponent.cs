using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
    /// <summary>
    /// Represent a MessageComponent that can be interacted with
    /// </summary>
    public class BaseInteractableComponent : BaseComponent
    {
        /// <summary>
        /// A developer-defined identifier for the interactable component
        /// Max 100 characters
        /// </summary>
        [JsonProperty("custom_id")]
        public string CustomId { get; set; }
    }
}