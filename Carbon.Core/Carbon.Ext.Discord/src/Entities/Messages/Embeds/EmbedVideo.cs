using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#embed-object-embed-video-structure">Embed Video Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class EmbedVideo
    {
        /// <summary>
        /// Source url of video
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// Source url of video
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Height of video
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Width of video
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Embed Video Constructor
        /// </summary>
        public EmbedVideo()
        {
            
        }
        
        /// <summary>
        /// Embed Video Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="proxyUrl"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public EmbedVideo(string url, int? height, int? width, string proxyUrl)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }
    }
}