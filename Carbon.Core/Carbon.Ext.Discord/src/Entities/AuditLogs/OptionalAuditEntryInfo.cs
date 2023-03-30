using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class OptionalAuditEntryInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("delete_member_days")]
        public string DeleteMemberDays { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("members_removed")]
        public string MembersRemoved { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("message_id")]
        public Snowflake MessageId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("count")]
        public string Count { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("role_name")]
        public string RoleName { get; set; }
    }
}
