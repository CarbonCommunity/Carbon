using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#request-guild-members">Request Guild Members</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildMembersRequestCommand
    {
        /// <summary>
        /// ID of the guild to get members for
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// String that username starts with, or an empty string to return all members
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; } = "";

        /// <summary>
        /// Maximum number of members to send matching the query;
        /// a limit of 0 can be used with an empty string query to return all members
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; } = 0;

        /// <summary>
        /// Used to specify if we want the presences of the matched members
        /// </summary>
        [JsonProperty("presences")]
        public bool? Presences { get; set; }
        
        /// <summary>
        /// Used to specify which users you wish to fetch
        /// </summary>
        [JsonProperty("user_ids")]
        public List<Snowflake> UserIds { get; set; }        
        
        /// <summary>
        /// Nonce to identify the Guild Members Chunk response (Up to 25 characters)
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
