using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels.Threads;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#thread-members-update-thread-members-update-event-fields">Thread Members Update Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThreadMembersUpdatedEvent
    {
        /// <summary>
        /// The id of the thread
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }     
        
        /// <summary>
        /// The approximate number of members in the thread, capped at 50
        /// </summary>
        [JsonProperty("member_count")]
        public int MemberCount { get; set; }
        
        /// <summary>
        /// The users who were added to the thread
        /// </summary>
        [JsonProperty("added_members")]
        public List<ThreadMember> AddedMembers { get; set; }
        
        /// <summary>
        /// The id of the users who were removed from the thread
        /// </summary>
        [JsonProperty("removed_member_ids")]
        public List<Snowflake> RemovedMemberIds { get; set; }
    }
}