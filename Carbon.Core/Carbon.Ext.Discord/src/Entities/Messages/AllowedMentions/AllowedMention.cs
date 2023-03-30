using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oxide.Ext.Discord.Entities.Messages.AllowedMentions
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#allowed-mentions-object">Allowed Mention Types</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AllowedMention
    {
        /// <summary>
        /// An array of allowed mention types to parse from the content.
        /// <see cref="AllowedMentionTypes"/>
        /// </summary>
        [JsonProperty("parse")]
        public List<AllowedMentionTypes> AllowedTypes { get; set; }
        
        /// <summary>
        /// Array of Role IDs to mention (Max size of 100)
        /// </summary>
        [JsonProperty("roles")]
        public List<Snowflake> Roles { get; set; }
        
        /// <summary>
        /// Array of User IDs to mention (Max size of 100)
        /// </summary>
        [JsonProperty("users")]
        public List<Snowflake> Users { get; set; }
        
        /// <summary>
        /// For replies, whether to mention the author of the message being replied to (default false)
        /// </summary>
        [JsonProperty("replied_user")]
        public bool RepliedUser { get; set; }
    }
}