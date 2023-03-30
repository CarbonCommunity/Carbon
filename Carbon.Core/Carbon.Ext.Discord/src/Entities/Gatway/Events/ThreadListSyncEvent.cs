using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#thread-list-sync-thread-list-sync-event-fields">Thread List Sync</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ThreadListSyncEvent
    {
        /// <summary>
        /// The ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
        
        /// <summary>
        /// The parent channel ids whose threads are being synced. If omitted, then threads were synced for the entire guild. This array may contain channel_ids that have no active threads as well, so you know to clear that data.
        /// </summary>
        [JsonProperty("channel_ids")]
        public List<Snowflake> ChannelIds { get; set; }
        
        /// <summary>
        ///	All active threads in the given channels that the current user can access
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordChannel>))]
        [JsonProperty("threads")]
        public Hash<Snowflake, DiscordChannel> Threads { get; set; }
        
        /// <summary>
        ///	All thread member objects from the synced threads for the current user, indicating which threads the current user has been added to
        /// </summary>
        [JsonProperty("members")]
        public List<ThreadMember> Members { get; set; }
    }
}