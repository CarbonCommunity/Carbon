using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Emojis
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/emoji#modify-guild-emoji-json-params">Emoji Update Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmojiUpdate
    {
        /// <summary>
        /// Emoji name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Roles this emoji is whitelisted to
        /// </summary>
        [JsonProperty("roles")]
        public List<Snowflake> Roles { get; set; }
    }
}