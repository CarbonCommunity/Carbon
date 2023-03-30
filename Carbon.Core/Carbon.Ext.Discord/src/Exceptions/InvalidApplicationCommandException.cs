namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Represents an invalid application command
    /// </summary>
    public class InvalidApplicationCommandException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidApplicationCommandException(string message) : base(message)
        {
            
        }
    }
}