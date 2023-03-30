using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#embed-object-embed-image-structure">Embed Image Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmbedImage
    {
        /// <summary>
        /// Source url of image (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// A proxied url of the image
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Height of image
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Width of image
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Embed Image Constructor
        /// </summary>
        public EmbedImage()
        {
            
        }
        
        /// <summary>
        /// Embed Image Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="proxyUrl"></param>
        public EmbedImage(string url, int? height = null, int? width = null, string proxyUrl = null)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }
    }
}