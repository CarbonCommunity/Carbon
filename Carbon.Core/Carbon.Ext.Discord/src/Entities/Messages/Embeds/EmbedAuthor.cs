using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#embed-object-embed-author-structure">Embed Author Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmbedAuthor
    {
        /// <summary>
        /// Name of author
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Url of author
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Url of author icon (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        /// <summary>
        /// A proxied url of author icon
        /// </summary>
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }

        /// <summary>
        /// Embed Author Constructor
        /// </summary>
        public EmbedAuthor()
        {
            
        }
        
        /// <summary>
        /// Embed Author Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="iconUrl"></param>
        /// <param name="proxyIconUrl"></param>
        public EmbedAuthor(string name, string url = null, string iconUrl = null, string proxyIconUrl = null)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }
    }
}