using System;

namespace Oxide.Ext.Discord.Logging
{
    /// <summary>
    /// Represents an interface for a logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a verbose message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Verbose(string message);
        
        /// <summary>
        /// Log a Debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);
        
        /// <summary>
        /// Log a Info message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);
        
        /// <summary>
        /// Log a Warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warning(string message);
        
        /// <summary>
        /// Log a Error message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Log a Exception message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="ex">Exception to log</param>
        void Exception(string message, Exception ex);

        /// <summary>
        /// Updates the log level for the current logger
        /// </summary>
        /// <param name="level">Level to update the logger to</param>
        void UpdateLogLevel(DiscordLogLevel level);

        /// <summary>
        /// Returns true if the logger is logging for the passed log level
        /// </summary>
        /// <param name="level">Log Level to check</param>
        /// <returns>True if the logger is logging for the given log level</returns>
        bool IsLogging(DiscordLogLevel level);
    }
}