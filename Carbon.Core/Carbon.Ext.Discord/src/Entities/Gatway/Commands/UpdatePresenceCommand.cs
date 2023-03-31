using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Activities;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#update-presence">Update Status</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UpdatePresenceCommand
    {
        /// <summary>
        /// The user's new status
        /// <see cref="UserStatusType"/>
        /// </summary>
        [JsonProperty("status")]
        public UserStatusType Status { get; set; } = UserStatusType.Online;

        /// <summary>
        /// The user's activities (Required)
        /// </summary>
        [JsonProperty("activities")]
        public List<DiscordActivity> Activities { get; set; }

        /// <summary>
        /// Unix time (in milliseconds) of when the client went idle, or null if the client is not idle
        /// </summary>
        [JsonProperty("since")]
        public int? Since { get; set; } = 0;

        /// <summary>
        /// Whether or not the client is afk
        /// </summary>
        [JsonProperty("afk")]
        public bool Afk { get; set; } = false;
    }
}
