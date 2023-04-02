using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#list-active-threads">Thread List Structure</a> within Discord.
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#list-public-archived-threads-response-body">Thread List Public Archived Structure</a> within Discord.
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#list-private-archived-threads-response-body">Thread List Private Archived Structure</a> within Discord.
    /// Represents a <a href="https://discord.com/developers/docs/resources/guild#list-active-threads">Thread List Private Archived Structure</a> within Discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThreadList
    {
        /// <summary>
        /// The active threads
        /// </summary>
        [JsonProperty("threads")]
        public List<DiscordChannel> Threads { get; set; } 
        
        /// <summary>
        /// A thread member object for each returned thread the current user has joined
        /// </summary>
        [JsonProperty("members")]
        public List<ThreadMember> Members { get; set; } 
        
        /// <summary>
        /// Whether there are potentially additional threads that could be returned on a subsequent call
        /// </summary>
        [JsonProperty("has_more")]
        public bool HasMore { get; set; } 
    }
}