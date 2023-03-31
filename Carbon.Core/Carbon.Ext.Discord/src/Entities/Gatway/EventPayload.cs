using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Gatway.Events;
using Oxide.Ext.Discord.WebSockets;

namespace Oxide.Ext.Discord.Entities.Gatway
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#payloads">Gateway Payload Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EventPayload
    {
        /// <summary>
        /// Op Code for the payload
        /// </summary>
        [JsonProperty("op")]
        public GatewayEventCode OpCode { get; internal set; }

        /// <summary>
        /// The event name for this payload
        /// </summary>
        [JsonProperty("t")]
        public JToken EventName { get; internal set; }

        /// <summary>
        /// Event data
        /// </summary>
        [JsonProperty("d")]
        public object Data { get; internal set;}

        /// <summary>
        /// Sequence number, used for resuming sessions and heartbeats
        /// </summary>
        [JsonProperty("s")]
        public int? Sequence { get; internal set; }

        /// <summary>
        /// Returns a DispatchCode enum value for the EventName if the extension supports it; Else the code will be Unknown
        /// </summary>
        public DispatchCode EventCode => EventName?.ToObject<DispatchCode>() ?? DispatchCode.Unknown;

        /// <summary>
        /// Data as JObject
        /// </summary>
        public JObject EventData => Data as JObject;

        /// <summary>
        /// Data as JToken
        /// </summary>
        public JToken TokenData => Data as JToken;
    }
}