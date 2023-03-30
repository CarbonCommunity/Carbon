using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#identify">Identify</a> Command
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IdentifyCommand
    {
        /// <summary>
        /// Authentication token
        /// </summary>
        [JsonProperty("token")]
        public string Token;

        /// <summary>
        /// Connection properties
        /// </summary>
        [JsonProperty("properties")]
        public Properties Properties;

        /// <summary>
        /// Whether this connection supports compression of packets
        /// </summary>
        [JsonProperty("compress")]
        public bool? Compress;

        /// <summary>
        /// Value between 50 and 250, total number of members where the gateway will stop sending offline members in the guild member list
        /// </summary>
        [JsonProperty("large_threshold")]
        public int? LargeThreshold;

        /// <summary>
        /// Used for Guild Sharding
        /// See <a href="https://discord.com/developers/docs/topics/gateway#sharding">Guild Sharding</a>
        /// </summary>
        [JsonProperty("shard")]
        public List<int> Shard;

        /// <summary>
        /// Presence structure for initial presence information
        /// </summary>
        [JsonProperty("presence")]
        public UpdatePresenceCommand PresenceUpdate;

        /// <summary>
        /// The Gateway Intents you wish to receive
        /// See <a href="https://discord.com/developers/docs/topics/gateway#gateway-intents">Gateway Intents</a>
        /// See <see cref="GatewayIntents"/>
        /// </summary>
        [JsonProperty("intents")]
        public GatewayIntents Intents { get; set; }
    }

    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#identify-identify-connection-properties">Identify Connection Properties</a>
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// Your operating system
        /// </summary>
        [JsonProperty("$os")]
        public string OS;

        /// <summary>
        /// Your library name
        /// </summary>
        [JsonProperty("$browser")]
        public string Browser;

        /// <summary>
        /// Your library name
        /// </summary>
        [JsonProperty("$device")]
        public string Device;
    }
}
