namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Represents an invalid embed
    /// </summary>
    public class InvalidEmbedException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidEmbedException(string message) : base(message)
        {
            
        }
    }
}