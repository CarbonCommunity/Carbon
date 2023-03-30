using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Integrations;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Users.Connections
{
    /// <summary>
    /// Represents a Discord Users <a href="https://discord.com/developers/docs/resources/user#connection-object">Connection</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Connection
    {
        /// <summary>
        /// ID of the connection account
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }

        /// <summary>
        /// The username of the connection account
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The service of the connection (twitch, youtube)
        /// <see cref="ConnectionType"/>
        /// </summary>
        [JsonProperty("type")]
        public ConnectionType Type { get; set; }

        /// <summary>
        /// Whether the connection is revoked
        /// </summary>
        [JsonProperty("revoked")]
        public bool? Revoked { get; set; }

        /// <summary>
        /// An array of partial server integrations
        /// <see cref="Integration"/>
        /// </summary>
        [JsonConverter(typeof(HashListConverter<Integration>))]
        [JsonProperty("integrations")]
        public Hash<Snowflake, Integration> Integrations { get; set; }
        
        /// <summary>
        /// Whether the connection is verified
        /// </summary>
        [JsonProperty("verified")]
        public bool Verified { get; set; }      
        
        /// <summary>
        /// Whether friend sync is enabled for this connection
        /// </summary>
        [JsonProperty("friend_sync")]
        public bool FriendSync { get; set; }        
        
        /// <summary>
        /// Whether activities related to this connection will be shown in presence updates
        /// </summary>
        [JsonProperty("show_activity")]
        public bool ShowActivity { get; set; }        
        
        /// <summary>
        /// Visibility of this connection
        /// <see cref="ConnectionVisibilityType"/>
        /// </summary>
        [JsonProperty("visibility")]
        public ConnectionVisibilityType Visibility { get; set; }
    }
}
