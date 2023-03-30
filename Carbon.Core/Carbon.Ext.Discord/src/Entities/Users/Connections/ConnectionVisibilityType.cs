namespace Oxide.Ext.Discord.Entities.Users.Connections
{
    /// <summary>
    /// Represents connection <a href="https://discord.com/developers/docs/resources/user#connection-object-visibility-types">Visibility Types</a>
    /// </summary>
    public enum ConnectionVisibilityType
    {
        /// <summary>
        /// Invisible to everyone except the user themselves
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Visible to everyone
        /// </summary>
        Everyone = 1
    }
}