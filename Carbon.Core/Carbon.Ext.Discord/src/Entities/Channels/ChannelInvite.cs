using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Invites;

namespace Oxide.Ext.Discord.Entities.Channels
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#create-channel-invite-json-params">Channel Invite Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChannelInvite
    {
        /// <summary>
        /// Duration of invite in seconds before expiry, or 0 for never
        /// </summary>
        [JsonProperty("max_age")]
        public int MaxAge { get; set; } = 86400;
        
        /// <summary>
        /// Max number of uses or 0 for unlimited
        /// </summary>
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }
        
        /// <summary>
        /// Whether this invite only grants temporary membership
        /// </summary>
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        
        /// <summary>
        /// If true, don't try to reuse a similar invite (useful for creating many unique one time use invites)
        /// </summary>
        [JsonProperty("unique")]
        public bool Unique { get; set; }
        
        /// <summary>
        /// The type of target user for this invite
        /// </summary>
        [JsonProperty("target_user_type")]
        public TargetUserType? TargetUserType { get; set; }
        
        /// <summary>
        /// The target user id for this invite
        /// </summary>
        [JsonProperty("target_user_id")]
        public Snowflake TargetUser { get; set; }
        
        /// <summary>
        /// The id of the embedded application to open for this invite,
        /// Required if TargetUserType is EMBEDDED_APPLICATION, the application must have the EMBEDDED flag	
        /// </summary>
        [JsonProperty("target_application_id")]
        public Snowflake TargetApplicationId { get; set; }
    }
}