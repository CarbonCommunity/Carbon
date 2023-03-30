namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Represents an invalid message component
    /// </summary>
    public class InvalidMessageComponentException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidMessageComponentException(string message) : base(message)
        {
            
        }
    }
}