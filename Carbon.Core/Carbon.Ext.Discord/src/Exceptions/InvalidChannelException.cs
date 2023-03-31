namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Represents using an invalid channel
    /// </summary>
    public class InvalidChannelException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public InvalidChannelException(string message): base(message)
        {
            
        }
    }
}