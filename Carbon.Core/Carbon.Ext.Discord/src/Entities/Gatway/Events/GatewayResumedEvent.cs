using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#resumed">Resumed</a>
    /// The resumed event is dispatched when a client has sent a resume payload to the gateway (for resuming existing sessions).
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GatewayResumedEvent
    {
        
    }
}
