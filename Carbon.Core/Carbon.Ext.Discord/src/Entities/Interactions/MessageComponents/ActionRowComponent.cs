using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/message-components#actionrow">Action Row Component</a> within discord
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ActionRowComponent : BaseComponent
    {
        /// <summary>
        /// The components on the action row
        /// </summary>
        [JsonConverter(typeof(MessageComponentsConverter))]
        [JsonProperty("components")]
        public List<BaseComponent> Components { get; } = new List<BaseComponent>();

        /// <summary>
        /// Constructor for ActionRowComponent
        /// Sets the Type to ActionRow
        /// </summary>
        public ActionRowComponent()
        {
            Type = MessageComponentType.ActionRow;
        }
    }
}