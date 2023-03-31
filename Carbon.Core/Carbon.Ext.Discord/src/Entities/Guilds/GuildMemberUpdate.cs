using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#modify-guild-member-json-params">Guild Member Update Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildMemberUpdate
    {
        /// <summary>
        /// The nickname to give the user
        /// Requires MANAGE_NICKNAMES Permission
        /// </summary>
        [JsonProperty("nick")]
        public string Nick { get; set; }
        
        /// <summary>
        /// New list of guild members roles
        /// Will replaces all roles with the ones in this list
        /// Requires MANAGE_ROLES Permission
        /// </summary>
        [JsonProperty("roles")]
        public List<Snowflake> Roles { get; set; }
        
        /// <summary>
        /// Deafen the guild member
        /// Requires DEAFEN_MEMBERS Permission
        /// </summary>
        [JsonProperty("deaf")]
        public bool? Deaf { get; set; }

        /// <summary>
        /// Mute the guild member
        /// Requires MUTE_MEMBERS Permission
        /// </summary>
        [JsonProperty("mute")]
        public bool? Mute { get; set; }
        
        /// <summary>
        /// The channel to move the user to
        /// Requires MOVE_MEMBERS Permission
        /// Setting to null will remove that member from a voice channel
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake? ChannelId { get; set; }
        
        /// <summary>
        /// When the user's timeout will expire and the user will be able to communicate in the guild again (up to 28 days in the future), set to null to remove timeout
        /// </summary>
        [JsonProperty("communication_disabled_until")]
        public DateTime? CommunicationDisabledUntil { get; set; }
    }
}