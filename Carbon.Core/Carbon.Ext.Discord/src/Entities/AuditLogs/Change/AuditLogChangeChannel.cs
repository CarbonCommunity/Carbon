using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Channels;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogChangeChannel
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("position")]
        public int? Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("bitrate")]
        public int? Bitrate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("permission_overwrites")]
        public List<Overwrite> PermissionOverwrites { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("application_id")]
        public Snowflake ApplicationId { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("rate_limit_per_user")]
        public int? RateLimitPerUser { get; set; }
    }
}