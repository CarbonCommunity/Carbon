using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Entities.Guilds;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#guild-members-chunk">Guild Members Chunk</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildMembersChunkEvent
    {
        /// <summary>
        /// The id of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// Set of guild members
        /// </summary>
        [JsonProperty("members")]
        public List<GuildMember> Members { get; set; }

        /// <summary>
        /// The chunk index in the expected chunks for this response (0 &lt;= chunk_index &lt; chunk_count)
        /// </summary>
        [JsonProperty("chunk_index")]
        public int ChunkIndex { get; set; }	
        
        /// <summary>
        /// The total number of expected chunks for this response
        /// </summary>
        [JsonProperty("chunk_count")]
        public int ChunkCount { get; set; }	
        
        /// <summary>
        /// If passing an invalid id to REQUEST_GUILD_MEMBERS, it will be returned here
        /// </summary>
        [JsonProperty("not_found")]
        public List<string> NotFound { get; set; }
        
        /// <summary>
        /// If passing true to REQUEST_GUILD_MEMBERS, presences of the returned members will be here
        /// </summary>
        [JsonProperty("presences")]
        public List<UpdatePresenceCommand> Presences { get; set; }      
        
        /// <summary>
        /// The nonce used in the Guild Members Request
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
