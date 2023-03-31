using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-choice-structure">ApplicationCommandOptionChoice</a>
    /// If you specify choices for an option, they are the only valid values for a user to pick
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CommandOptionChoice
    {
        /// <summary>
        /// Choice name (1-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Type can be string, integer, double or boolean
        /// Value of the choice, up to 100 characters if string
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}