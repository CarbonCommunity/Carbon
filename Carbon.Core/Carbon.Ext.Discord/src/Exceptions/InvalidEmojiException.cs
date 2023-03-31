namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Error thrown when an emoji string fails validation
    /// </summary>
    public class InvalidEmojiException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emojiValue">Value for the emoji</param>
        /// <param name="validationError">Validation error message</param>
        internal InvalidEmojiException(string emojiValue, string validationError) : base($"'{emojiValue}' failed emoji validation with error: {validationError}")
        {
            
        }
    }
}