using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Activities;
using Oxide.Ext.Discord.Entities.Gatway.Commands;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#presence-update">Presence Update</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PresenceUpdatedEvent
    {
        /// <summary>
        /// The user presence is being updated for
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; }

        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }
        
        /// <summary>
        /// Users status
        /// </summary>
        [JsonProperty("status")]
        public UserStatusType Status { get; set; }
        
        /// <summary>
        /// User's current activities
        /// </summary>
        [JsonProperty("activities")]
        public List<DiscordActivity> Activities { get; set; }
        
        /// <summary>
        /// User's platform-dependent status
        /// </summary>
        [JsonProperty("client_status")]
        public ClientStatus ClientStatus { get; set; }
    }
}
