namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Represents an invalid message
    /// </summary>
    public class InvalidMessageException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidMessageException(string message) : base(message)
        {
            
        }
    }
}