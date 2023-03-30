namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/interactions/message-components#component-types">Message Component Type</a> within Discord..
    /// </summary>
    public enum MessageComponentType
    {
        /// <summary>
        /// A container for other components
        /// </summary>
        ActionRow = 1,
        
        /// <summary>
        /// A clickable button
        /// </summary>
        Button = 2,
        
        /// <summary>
        /// A select menu for picking from choices
        /// </summary>
        SelectMenu = 3,
        
        /// <summary>
        /// A text box for inserting written responses
        /// </summary>
        InputText = 4
    }
}