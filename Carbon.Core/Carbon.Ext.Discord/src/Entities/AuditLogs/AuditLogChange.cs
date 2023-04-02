using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.AuditLogs.Change;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogChange
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("new_value")]
        public AuditLogChangeBase NewValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("old_value")]
        public AuditLogChangeBase OldValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
