using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogEntry
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("target_id")]
        public Snowflake TargetId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("changes")]
        public List<AuditLogChange> Changes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("user_id")]
        public Snowflake? UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("action_type")]
        public AuditLogActionType? ActionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("options")]
        public OptionalAuditEntryInfo Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
