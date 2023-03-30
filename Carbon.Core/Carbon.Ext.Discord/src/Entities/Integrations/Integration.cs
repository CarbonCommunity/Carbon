using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Integrations
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#integration-object">Integration Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Integration : IntegrationUpdate, ISnowflakeEntity
    {
        /// <summary>
        /// Integration ID
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }

        /// <summary>
        /// Integration Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Integration type
        /// See <see cref="IntegrationType"/>
        /// </summary>
        [JsonProperty("type")]
        public IntegrationType Type { get; set; }

        /// <summary>
        /// Is this integration enabled
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Is this integration syncing
        /// </summary>
        [JsonProperty("syncing")]
        public bool? Syncing { get; set; }

        /// <summary>
        /// ID that this integration uses for "subscribers"
        /// </summary>
        [JsonProperty("role_id")]
        public Snowflake? RoleId { get; set; }

        /// <summary>
        /// User for this integration
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; }

        /// <summary>
        /// Integration account information
        /// </summary>
        [JsonProperty("account")]
        public Account Account { get; set; }

        /// <summary>
        /// When this integration was last synced
        /// </summary>
        [JsonProperty("synced_at")]
        public DateTime? SyncedAt { get; set; }
        
        /// <summary>
        /// How many subscribers this integration has
        /// </summary>
        [JsonProperty("subscriber_count")]
        public int? SubscriberCount { get; set; }
        
        /// <summary>
        /// Has this integration been revoked
        /// </summary>
        [JsonProperty("revoked")]
        public bool? Revoked { get; set; }
        
        /// <summary>
        /// The bot/OAuth2 application for discord integrations
        /// </summary>
        [JsonProperty("application")]
        public IntegrationApplication Application { get; set; }
    }
}
