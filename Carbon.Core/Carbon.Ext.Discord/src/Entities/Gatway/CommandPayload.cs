using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Gatway.Commands;

namespace Oxide.Ext.Discord.Entities.Gatway
{
    /// <summary>
    /// Represents a command payload
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CommandPayload
    {
        /// <summary>
        /// Command Code for the payload
        /// </summary>
        [JsonProperty("op")]
        public GatewayCommandCode OpCode;

        /// <summary>
        /// Payload data
        /// </summary>
        [JsonProperty("d")]
        public object Payload;
    }
}
