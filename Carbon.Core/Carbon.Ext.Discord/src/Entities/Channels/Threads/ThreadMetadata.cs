using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels.Threads
{
    /// <summary>
    /// Represents a guild or DM <a href="https://discord.com/developers/docs/resources/channel#thread-metadata-object-thread-metadata-structure">Thread Metadata Structure</a> within Discord.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThreadMetadata
    {
        /// <summary>
        /// Whether the thread is archived
        /// </summary>
        [JsonProperty("archived")]
        public bool Archived { get; set; }

        /// <summary>
        /// Duration in minutes to automatically archive the thread after recent activity, can be set to: 60, 1440, 4320, 10080
        /// </summary>
        [JsonProperty("auto_archive_duration")]
        public int AutoArchiveDuration { get; set; } 
        
        /// <summary>
        /// Timestamp when the thread's archive status was last changed, used for calculating recent activity
        /// </summary>
        [JsonProperty("archive_timestamp")]
        public DateTime ArchiveTimestamp { get; set; } 
        
        /// <summary>
        /// Whether the thread is locked
        /// When a thread is locked, only users with MANAGE_THREADS can unarchive it
        /// </summary>
        [JsonProperty("locked")]
        public bool? Locked { get; set; } 
        
        /// <summary>
        /// Whether non-moderators can add other non-moderators to a thread
        /// Only available on private threads
        /// </summary>
        [JsonProperty("invitable")]
        public bool? Invitable { get; set; } 
        
        /// <summary>
        /// Timestamp when the thread was created; only populated for threads created after 2022-01-09
        /// </summary>
        [JsonProperty("create_timestamp")]
        public DateTime? CreateTimestamp { get; set; } 
    }
}