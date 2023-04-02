using System;
using Oxide.Core;

namespace Oxide.Ext.Discord.Logging
{
    /// <summary>
    /// Represents a discord extension logger
    /// </summary>
    public class Logger : ILogger
    {
        private DiscordLogLevel _logLevel;
        
        /// <summary>
        /// Creates a new logger with the given log level
        /// </summary>
        /// <param name="logLevel">Log level of the logger</param>
        public Logger(DiscordLogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        /// <summary>
        /// Log a verbose message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Verbose(string message)
        {
            Log(DiscordLogLevel.Verbose, message);
        }

        /// <summary>
        /// Log a Debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
            Log(DiscordLogLevel.Debug, message);
        }
        
        /// <summary>
        /// Log a Info message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Info(string message)
        {
            Log(DiscordLogLevel.Info, message);
        }
        
        /// <summary>
        /// Log a Warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warning(string message)
        {
            Log(DiscordLogLevel.Warning, message);
        }
        
        /// <summary>
        /// Log a Error message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
            Log(DiscordLogLevel.Error, message);
        }

        /// <summary>
        /// Log a Exception message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="ex">Exception to log</param>
        public void Exception(string message, Exception ex)
        {
            Log(DiscordLogLevel.Exception, message, ex);
        }

        private void Log(DiscordLogLevel level, string message, object data = null)
        {
            if (!IsLogging(level))
            {
                return;
            }

            string log = $"[Discord Extension] [{level.ToString()}]: {message}";
            switch (level)
            {
                case DiscordLogLevel.Debug:
                case DiscordLogLevel.Warning:
                    Interface.Oxide.LogWarning(log);
                    break;
                case DiscordLogLevel.Error:
                    Interface.Oxide.LogError(log);
                    break;
                case DiscordLogLevel.Exception:
                    Interface.Oxide.LogException($"{log}\n{data}", (Exception)data);
                    break;
                default:
                    Interface.Oxide.LogInfo(log);
                    break;
            }
        }

        /// <summary>
        /// Updates the log level for the current logger
        /// </summary>
        /// <param name="level">Level to update the logger to</param>
        public void UpdateLogLevel(DiscordLogLevel level)
        {
            _logLevel = level;
        }

        /// <summary>
        /// Returns true if the logger is logging for the passed log level
        /// </summary>
        /// <param name="level">Log Level to check</param>
        /// <returns>True if the logger is logging for the given log level</returns>
        public bool IsLogging(DiscordLogLevel level)
        {
            return level >= _logLevel;
        }
    }
}