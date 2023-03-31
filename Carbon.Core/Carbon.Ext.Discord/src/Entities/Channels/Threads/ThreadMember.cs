using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
    /// <summary>
    /// Represents a guild or DM <a href="https://discord.com/developers/docs/resources/channel#thread-member-object">Thread Member Structure</a> within Discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThreadMember
    {
        /// <summary>
        /// The id of the thread
        /// </summary>
        [JsonProperty("id")]
        public Snowflake? Id { get; set; } 
        
        /// <summary>
        /// The id of the user
        /// </summary>
        [JsonProperty("user_id")]
        public Snowflake? UserId { get; set; } 
        
        /// <summary>
        /// The time the current user last joined the thread
        /// </summary>
        [JsonProperty("join_timestamp")]
        public DateTime JoinTimestamp { get; set; } 
        
        /// <summary>
        /// Any user-thread settings, currently only used for notifications
        /// </summary>
        //TODO: Move to Enum if one becomes public
        [JsonProperty("flags")]
        public int Flags { get; set; }

        internal void Update(ThreadMember update)
        {
            Flags = update.Flags;
        }
    }
}