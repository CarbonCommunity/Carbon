using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Teams
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/teams#data-models-team-members-object">Team Members Object</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TeamMember
    {
        /// <summary>
        /// The user's membership state on the team
        /// </summary>
        [JsonProperty("membership_state")]
        public TeamMembershipState MembershipState { get; set; }
        
        /// <summary>
        /// The teams permissions
        /// Will always be ["*"]
        /// </summary>
        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }
        
        /// <summary>
        /// The id of the parent team of which they are a member
        /// </summary>
        [JsonProperty("team_id")]
        public Snowflake TeamId { get; set; }
        
        /// <summary>
        /// The avatar, discriminator, id, and username of the user
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; } 
    }
}