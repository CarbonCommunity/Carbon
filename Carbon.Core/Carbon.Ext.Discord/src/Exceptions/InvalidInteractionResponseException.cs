namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Error thrown when an interaction response is invalid
    /// </summary>
    public class InvalidInteractionResponseException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidInteractionResponseException(string message) : base(message) { }
    }
}