namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Exception throw when an attachment filename contains invalid characters
    /// </summary>
    public class InvalidFileNameException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">invalid file name</param>
        public InvalidFileNameException(string fileName) : base($"'{fileName}' is not a valid filename for discord. " +
                                                                "Valid filename characters are alphanumeric with underscores, dashes, or dots") 
        { }
    }
}