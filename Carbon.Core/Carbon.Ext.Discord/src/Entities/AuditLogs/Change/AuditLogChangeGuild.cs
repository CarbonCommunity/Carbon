using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLogChangeGuild
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("icon_hash")]
        public string IconHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("splash_hash")]
        public string SplashHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("owner_id")]
        public Snowflake OwnerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("afk_channel_id")]
        public Snowflake AfkChannelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("afk_timeout")]
        public int? AfkTimeout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("mfa_level")]
        public int? MfaLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("verification_level")]
        public int? VerificationLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("explicit_content_filter")]
        public int? ExplicitContentFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("default_message_notifications")]
        public int? DefaultMessageNotifications { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("vanity_url_code")]
        public string VanityUrlCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("$add")]
        public List<DiscordRole> Add { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("$remove")]
        public List<DiscordRole> Remove { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("prune_delete_days")]
        public int? PruneDeleteDays { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("widget_enabled")]
        public bool WidgetEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("widget_channel_id")]
        public Snowflake WidgetChannelId { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("system_channel_id")]
        public Snowflake SystemChannelId { get; set; }
    }
}