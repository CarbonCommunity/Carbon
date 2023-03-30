using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Integrations;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Webhooks;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuditLog
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("webhooks")]
        public List<DiscordWebhook> Webhooks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("users")]
        public List<DiscordUser> Users { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("audit_log_entries")]
        public List<AuditLogEntry> AuditLogEntries { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("integrations")]
        public List<Integration> Integrations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static void GetGuildAuditLog(DiscordClient client, DiscordGuild guild, Action<AuditLog> callback = null) => GetGuildAuditLog(client, guild.Id, callback);

        /// <summary>
        /// 
        /// </summary>
        public static void GetGuildAuditLog(DiscordClient client, Snowflake guildId, Action<AuditLog> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/audit-logs", RequestMethod.GET, null, callback, error);
        }
    }
}
