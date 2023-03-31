using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Interactions
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-resolved-data-structure">Application Command Interaction Data Option</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InteractionDataResolved
    {
        /// <summary>
        /// The IDs and User objects
        /// </summary>
        [JsonProperty("users")]
        public Hash<Snowflake, DiscordUser> Users { get; set; }
        
        /// <summary>
        /// The IDs and partial Member objects
        /// </summary>
        [JsonProperty("members")]
        public Hash<Snowflake, GuildMember> Members { get; set; }
        
        /// <summary>
        /// The IDs and Role objects
        /// </summary>
        [JsonProperty("roles")]
        public Hash<Snowflake, DiscordRole> Roles { get; set; }
        
        /// <summary>
        /// The IDs and partial Channel objects
        /// </summary>
        [JsonProperty("channels")]
        public Hash<Snowflake, DiscordChannel> Channels { get; set; }
        
        /// <summary>
        /// The ids and partial Message objects
        /// </summary>
        [JsonProperty("messages")]
        public Hash<Snowflake, DiscordMessage> Messages { get; set; }
    }
}