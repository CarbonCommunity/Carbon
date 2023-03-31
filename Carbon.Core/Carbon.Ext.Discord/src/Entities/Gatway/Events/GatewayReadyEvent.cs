using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Applications;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Interactions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#ready">Ready</a>
    /// The ready event is dispatched when a client has completed the initial handshake with the gateway (for new sessions)
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GatewayReadyEvent
    {
        /// <summary>
        /// Gateway version
        /// See <a href="https://discord.com/developers/docs/topics/gateway#gateways-gateway-versions">Gateway Version</a>
        /// </summary>
        [JsonProperty("v")]
        public int Version { get; private set; }
        
        /// <summary>
        /// Information about the user including email
        /// See <see cref="DiscordUser"/>
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; }

        /// <summary>
        /// The guilds the user is in
        /// </summary>
        [JsonProperty("guilds")]
        [JsonConverter(typeof(HashListConverter<DiscordGuild>))]
        public Hash<Snowflake, DiscordGuild> Guilds { get; set; }

        /// <summary>
        /// Used for resuming connections
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// The shard information associated with this session, if sent when identifying
        /// </summary>
        [JsonProperty("shard")]
        public List<int> Shard { get; set; }
        
        /// <summary>
        /// Contains id and flags
        /// See <see cref="Application"/>
        /// </summary>
        [JsonProperty("application")]
        public DiscordApplication Application { get; set; }
    }
}
