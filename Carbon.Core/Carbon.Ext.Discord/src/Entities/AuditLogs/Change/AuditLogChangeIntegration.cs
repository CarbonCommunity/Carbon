using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogChangeIntegration
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enable_emoticons")]
        public bool EnableEmoticons { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("expire_behavior")]
        public int ExpireBehavior { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("expire_grace_period")]
        public int ExpireGracePeriod { get; set; }
    }
}