namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Represents an invalid discord color
    /// </summary>
    public class InvalidDiscordColorException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidDiscordColorException(string message) : base(message)
        {
            
        }
    }
}