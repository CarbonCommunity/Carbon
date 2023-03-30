namespace Oxide.Ext.Discord.Logging
{
    /// <summary>
    /// Represents the log level for a logger
    /// </summary>
    public enum DiscordLogLevel
    {
        /// <summary>
        /// Verbose log level displays all message
        /// </summary>
        Verbose,
        
        /// <summary>
        /// Debug log level display all messages up to and include Debug
        /// </summary>
        Debug,
        
        /// <summary>
        /// Info log level display all messages up to and include Info
        /// </summary>
        Info,
        
        /// <summary>
        /// Warning log level display all messages up to and include Warning
        /// </summary>
        Warning,
        
        /// <summary>
        /// Error log level display all messages up to and include Error
        /// </summary>
        Error,
        
        /// <summary>
        /// Exception log level display all messages up to and include Exception
        /// </summary>
        Exception,
        
        /// <summary>
        /// Disables all logging
        /// </summary>
        Off
    }
}