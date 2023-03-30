using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Emojis
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/emoji#create-guild-emoji-json-params">Emoji Create Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmojiCreate : EmojiUpdate
    {
        /// <summary>
        /// The 128x128 emoji image
        /// Emojis and animated emojis have a maximum file size of 256kb.
        /// Attempting to upload an emoji larger than this limit will fail and return 400 Bad Request
        /// </summary>
        [JsonProperty("image")]
        public string ImageData { get; set; }
    }
}