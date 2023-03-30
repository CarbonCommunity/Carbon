using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Invites
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/invite#invite-metadata-object-invite-metadata-structure">Invite Metadata Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InviteMetadata : DiscordInvite
    {
        /// <summary>
        /// Number of times this invite has been used
        /// </summary>
        [JsonProperty("uses")]
        public int Uses { get; set; }
        
        /// <summary>
        /// Max number of times this invite can be used
        /// </summary>
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }
        
        /// <summary>
        /// Duration (in seconds) after which the invite expires
        /// </summary>
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }
        
        /// <summary>
        /// Whether this invite only grants temporary membership
        /// </summary>
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        
        /// <summary>
        /// When this invite was created
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}