using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#resume">Resume</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ResumeSessionCommand
    {
        /// <summary>
        /// Session token
        /// </summary>
        [JsonProperty("token")]
        public string Token;

        /// <summary>
        /// Session ID
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId;

        /// <summary>
        /// Last sequence number received
        /// </summary>
        [JsonProperty("seq")]
        public int Sequence;
    }
}
