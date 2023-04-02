using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogChangeRole
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("permissions")]
        public string Permissions { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("color")]
        public DiscordColor Color { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("hoist")]
        public bool Hoist { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("mentionable")]
        public bool Mentionable { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("allow")]
        public int? Allow { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("deny")]
        public int? Deny { get; set; }
    }
}