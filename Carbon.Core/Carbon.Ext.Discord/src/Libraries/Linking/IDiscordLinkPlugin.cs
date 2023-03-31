using System.Collections.Generic;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Libraries.Linking
{
    /// <summary>
    /// Represents a plugin that supports Discord Link library
    /// </summary>
    public interface IDiscordLinkPlugin
    {
        /// <summary>
        /// Title of the plugin
        /// </summary>
        string Title { get; }
        
        /// <summary>
        /// Returns a <see cref="IDictionary{TKey,TValue}"/> of Steam ID's to Discord ID's
        /// </summary>
        /// <returns></returns>
        IDictionary<string, Snowflake> GetSteamToDiscordIds();
    }
}