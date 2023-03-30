using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#add-guild-member-json-params">Guild Member Add</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildMemberAdd
    {
        /// <summary>
        /// An oauth2 access token granted with the guilds.join to the bot's application for the user you want to add to the guild
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Value to set users nickname to
        /// Requires permission MANAGE_NICKNAMES
        /// </summary>
        [JsonProperty("nick")]
        public string Nick { get; set; }
        
        /// <summary>
        /// Role ids the member is assigned
        /// Requires permission MANAGE_ROLES
        /// </summary>
        [JsonProperty("roles")]
        public List<Snowflake> Roles { get; set; }
        
        /// <summary>
        /// Whether the user is muted in voice channels
        /// Requires permission MUTE_MEMBERS
        /// </summary>
        [JsonProperty("mute")]
        public bool Mute { get; set; }

        /// <summary>
        /// Whether the user is deafened in voice channels
        /// Requires permission DEAFEN_MEMBERS
        /// </summary>
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
    }
}