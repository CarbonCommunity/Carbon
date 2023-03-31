using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#modify-channel-json-params-group-dm">Group DM Channel Update Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GroupDmChannelUpdate
    {
        /// <summary>
        /// The name of the channel (2-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Base64 encoded icon
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}