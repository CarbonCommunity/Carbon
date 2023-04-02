using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogChangeInvite
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("inviter_id")]
        public Snowflake InviterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("max_uses")]
        public int? MaxUses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("uses")]
        public int? Uses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("max_age")]
        public int? MaxAge { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
    }
}