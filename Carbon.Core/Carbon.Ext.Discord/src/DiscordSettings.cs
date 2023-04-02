using System.Text.RegularExpressions;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Logging;

namespace Oxide.Ext.Discord
{
    /// <summary>
    /// Represents settings used to connect to discord
    /// </summary>
    public class DiscordSettings
    {
        /// <summary>
        /// API token for the bot
        /// </summary>
        public string ApiToken;

        /// <summary>
        /// Discord Extension Logging Level.
        /// See <see cref="LogLevel"/>
        /// </summary>
        public DiscordLogLevel LogLevel = DiscordLogLevel.Info;
        
        /// <summary>
        /// Intents that your bot needs to work
        /// See <see cref="GatewayIntents"/>
        /// </summary>
        public GatewayIntents Intents = GatewayIntents.None;

        private static readonly Regex HideTokenRegex = new Regex("\\w|-", RegexOptions.Compiled);
        
        /// <summary>
        /// Hides the token but keeps the format to allow for debugging token issues without showing the token.
        /// </summary>
        /// <returns></returns>
        public string GetHiddenToken()
        {
            return $"\"{HideTokenRegex.Replace(ApiToken, "#")}\"";
        }
    }
}